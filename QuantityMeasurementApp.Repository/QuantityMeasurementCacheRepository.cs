using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using QuantityMeasurementApp.Entity;

namespace QuantityMeasurementApp.Repository
{
    public class QuantityMeasurementCacheRepository : IQuantityMeasurementRepository
    {
        // Singleton instance
        private static QuantityMeasurementCacheRepository _instance;

        // Lock object for thread safety
        private static readonly object _lock = new object();

        // In-memory cache
        private List<QuantityMeasurementEntity> _cache;

        // JSON file path — acts as persistent database
        private string _jsonFilePath = "measurement_history.json";

        // Private constructor — prevents outside instantiation
        private QuantityMeasurementCacheRepository()
        {
            _cache = new List<QuantityMeasurementEntity>();
            LoadFromJson();
        }

        // Thread-safe double-checked locking singleton
        public static QuantityMeasurementCacheRepository GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new QuantityMeasurementCacheRepository();
                    }
                }
            }
            return _instance;
        }

        // Saves entity to in-memory cache and persists to JSON
        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity", "Entity cannot be null");

            _cache.Add(entity);
            AppendToJson(entity);
        }

        // Returns all saved entities from cache
        public IList<QuantityMeasurementEntity> GetAll()
        {
            return _cache;
        }

        // Clears in-memory cache and deletes JSON file safely
        public void Clear()
        {
            _cache.Clear();

            try
            {
                if (File.Exists(_jsonFilePath))
                    File.Delete(_jsonFilePath);
            }
            catch (IOException)
            {
                // File is locked by another process — truncate instead
                try
                {
                    File.WriteAllText(_jsonFilePath, string.Empty);
                }
                catch
                {
                    // best effort — do not crash the test setup
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Access denied — truncate instead
                try
                {
                    File.WriteAllText(_jsonFilePath, string.Empty);
                }
                catch
                {
                    // best effort — do not crash the test setup
                }
            }
        }

        // ── JSON PERSISTENCE ──────────────────────────────────

        // Appends one entity as a JSON line (newline-delimited JSON)
        private void AppendToJson(QuantityMeasurementEntity entity)
        {
            try
            {
                string jsonLine = ToJsonLine(entity);
                File.AppendAllText(_jsonFilePath, jsonLine + Environment.NewLine, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Repository] Could not write to JSON: " + ex.Message);
            }
        }

        // On startup reads JSON file and reports how many records exist
        private void LoadFromJson()
        {
            try
            {
                if (File.Exists(_jsonFilePath))
                {
                    string[] lines = File.ReadAllLines(_jsonFilePath, Encoding.UTF8);
                    int loaded = 0;

                    foreach (string line in lines)
                    {
                        if (line.Trim().Length > 0)
                            loaded++;
                    }

                    Console.WriteLine("[Repository] JSON database found. Loaded "
                        + loaded + " record(s) from disk.");
                }
                else
                {
                    Console.WriteLine("[Repository] No existing JSON database. Starting fresh.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[Repository] Could not load JSON: " + ex.Message);
            }
        }

        // Converts entity to a single JSON line — no external library needed
        private string ToJsonLine(QuantityMeasurementEntity entity)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");

            sb.Append("\"timestamp\":\"");
            sb.Append(entity.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.Append("\",");

            sb.Append("\"operation\":\"");
            sb.Append(entity.Operation.ToString());
            sb.Append("\",");

            sb.Append("\"hasError\":");
            sb.Append(entity.HasError ? "true" : "false");

            if (entity.HasError)
            {
                sb.Append(",\"errorMessage\":\"");
                sb.Append(Escape(entity.ErrorMessage));
                sb.Append("\"");
            }

            if (entity.Operand1 != null)
            {
                sb.Append(",\"operand1\":{");
                sb.Append("\"value\":" + entity.Operand1.Value + ",");
                sb.Append("\"unit\":\"" + entity.Operand1.Unit + "\",");
                sb.Append("\"type\":\"" + entity.Operand1.MeasurementType + "\"");
                sb.Append("}");
            }

            if (entity.Operand2 != null)
            {
                sb.Append(",\"operand2\":{");
                sb.Append("\"value\":" + entity.Operand2.Value + ",");
                sb.Append("\"unit\":\"" + entity.Operand2.Unit + "\",");
                sb.Append("\"type\":\"" + entity.Operand2.MeasurementType + "\"");
                sb.Append("}");
            }

            if (entity.Operation == OperationType.COMPARE)
            {
                sb.Append(",\"result\":");
                sb.Append(entity.ComparisonResult ? "true" : "false");
            }
            else if (entity.Operation == OperationType.DIVIDE)
            {
                sb.Append(",\"result\":");
                sb.Append(entity.ScalarResult);
            }
            else if (entity.Result != null)
            {
                sb.Append(",\"result\":{");
                sb.Append("\"value\":" + entity.Result.Value + ",");
                sb.Append("\"unit\":\"" + entity.Result.Unit + "\"");
                sb.Append("}");
            }

            sb.Append("}");
            return sb.ToString();
        }

        // Escapes special characters for JSON string values
        private string Escape(string value)
        {
            if (value == null)
                return "";

            return value.Replace("\\", "\\\\")
                        .Replace("\"", "\\\"")
                        .Replace("\n", "\\n")
                        .Replace("\r", "\\r");
        }
    }
}