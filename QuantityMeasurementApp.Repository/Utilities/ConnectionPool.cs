using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.SqlClient;
using QuantityMeasurementApp.Repository.Utilities;

namespace QuantityMeasurementApp.Repository.Utilities
{
    public class ConnectionPool : IDisposable
    {
        private static ConnectionPool? _instance;

        public static ConnectionPool Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ConnectionPool(AppConfig.Instance);
                return _instance;
            }
        }

        private readonly string _connectionString;
        private readonly int _poolSize;
        private readonly Stack<SqlConnection> _available;
        private bool _disposed = false;

        private ConnectionPool(AppConfig config)
        {
            _connectionString = config.ConnectionString;
            _poolSize = config.PoolSize;
            _available = new Stack<SqlConnection>(_poolSize);

            Console.WriteLine("[ConnectionPool] Initialising...");

            EnsureDatabaseExists();
            InitialisePool();
            EnsureSchemaExists();

            Console.WriteLine("[ConnectionPool] Ready — " + _poolSize + " connections created.");
        }

        // Get a connection from the pool
        public SqlConnection AcquireConnection()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(ConnectionPool));

            if (_available.Count == 0)
                throw new InvalidOperationException(
                    "[ConnectionPool] No connections available. Pool size: " + _poolSize);

            SqlConnection connection = _available.Pop();

            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            return connection;
        }

        // Return a connection back to the pool
        public void ReleaseConnection(SqlConnection connection)
        {
            if (_disposed || connection == null)
                return;

            _available.Push(connection);
        }

        // Pool statistics
        public string GetPoolStatistics()
        {
            int available = _available.Count;
            int inUse = _poolSize - available;

            return "[ConnectionPool Stats] " +
                   "Total: " + _poolSize + " | " +
                   "Available: " + available + " | " +
                   "In Use: " + inUse;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            while (_available.Count > 0)
            {
                SqlConnection conn = _available.Pop();
                try { conn.Close(); conn.Dispose(); } catch { }
            }

            _instance = null;
            Console.WriteLine("[ConnectionPool] Disposed — all connections closed.");
        }

        // Creates the database on LocalDB if it does not exist
        private void EnsureDatabaseExists()
        {
            SqlConnectionStringBuilder builder =
                new SqlConnectionStringBuilder(_connectionString);

            string targetDb = builder.InitialCatalog;
            builder.InitialCatalog = "master";

            using SqlConnection masterConn =
                new SqlConnection(builder.ConnectionString);
            masterConn.Open();

            string sql = $@"
                IF NOT EXISTS (
                    SELECT name FROM sys.databases WHERE name = '{targetDb}'
                )
                CREATE DATABASE [{targetDb}]";

            using SqlCommand cmd = new SqlCommand(sql, masterConn);
            cmd.ExecuteNonQuery();

            Console.WriteLine("[ConnectionPool] Database '" + targetDb + "' ready.");
        }

        // Pre-creates all connections
        private void InitialisePool()
        {
            for (int i = 0; i < _poolSize; i++)
            {
                SqlConnection conn = new SqlConnection(_connectionString);
                conn.Open();
                _available.Push(conn);
            }
        }

        // Finds and runs QuantityMeasurementAppSchema.sql from solution root
        private void EnsureSchemaExists()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            string? schemaPath = FindSchemaFile(basePath, "QuantityMeasurementAppSchema.sql");

            Console.WriteLine("[ConnectionPool] Schema path: " + (schemaPath ?? "NOT FOUND"));

            if (schemaPath == null)
            {
                Console.WriteLine("[ConnectionPool] Warning: schema file not found.");
                return;
            }

            // Execute the entire schema as one command — no GO splitting needed
            string schemaSql = File.ReadAllText(schemaPath);
            SqlConnection conn = _available.Peek();

            try
            {
                using SqlCommand cmd = new SqlCommand(schemaSql, conn);
                cmd.ExecuteNonQuery();
                Console.WriteLine("[ConnectionPool] Schema initialised successfully.");
            }
            catch (SqlException ex)
            {
                Console.WriteLine("[ConnectionPool] Schema error: " + ex.Message);
                Console.WriteLine("[ConnectionPool] Run QuantityMeasurementAppSchema.sql manually.");
            }
        }
        // Walks up directory tree to find the schema file
        private string? FindSchemaFile(string startPath, string fileName)
        {
            DirectoryInfo? dir = new DirectoryInfo(startPath);

            while (dir != null)
            {
                string candidate = Path.Combine(dir.FullName, fileName);
                if (File.Exists(candidate))
                    return candidate;
                dir = dir.Parent;
            }

            return null;
        }
    }
}