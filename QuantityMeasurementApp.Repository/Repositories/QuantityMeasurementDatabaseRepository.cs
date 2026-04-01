using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Repository.Utilities;
using QuantityMeasurementApp.Repository.Exceptions;
using QuantityMeasurementApp.Entity.Enums;

namespace QuantityMeasurementApp.Repository.Repositories
{
    public class QuantityMeasurementDatabaseRepository : IQuantityMeasurementRepository
    {
        private readonly ConnectionPool _pool;

        public QuantityMeasurementDatabaseRepository()
        {
            _pool = ConnectionPool.Instance;
            Console.WriteLine("[DatabaseRepository] Initialised with connection pool.");
        }

        // ── SAVE ──────────────────────────────────────────────

        public void Save(QuantityMeasurementEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            SqlConnection conn = _pool.AcquireConnection();

            try
            {
                string sql = @"
                    INSERT INTO quantity_measurement_entity (
                        operand1_value, operand1_unit, operand1_type,
                        operand2_value, operand2_unit, operand2_type,
                        operation,
                        result_value,   result_unit,   result_type,
                        comparison_result,
                        scalar_result,
                        has_error,
                        error_message,
                        timestamp
                    )
                    VALUES (
                        @op1Val, @op1Unit, @op1Type,
                        @op2Val, @op2Unit, @op2Type,
                        @operation,
                        @resVal, @resUnit, @resType,
                        @compResult,
                        @scalarResult,
                        @hasError,
                        @errorMessage,
                        @timestamp
                    )";

                using SqlCommand cmd = new SqlCommand(sql, conn);

                // Operand 1 — always present
                cmd.Parameters.AddWithValue("@op1Val",  entity.Operand1Value);
                cmd.Parameters.AddWithValue("@op1Unit", entity.Operand1Unit);
                cmd.Parameters.AddWithValue("@op1Type", entity.Operand1MeasurementType);

                // Operand 2 — nullable (not present for CONVERT)
                if (entity.Operand2Value.HasValue)
                {
                    cmd.Parameters.AddWithValue("@op2Val",  entity.Operand2Value.Value);
                    cmd.Parameters.AddWithValue("@op2Unit", entity.Operand2Unit   ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@op2Type", entity.Operand2MeasurementType ?? (object)DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@op2Val",  DBNull.Value);
                    cmd.Parameters.AddWithValue("@op2Unit", DBNull.Value);
                    cmd.Parameters.AddWithValue("@op2Type", DBNull.Value);
                }

                // Operation
                cmd.Parameters.AddWithValue("@operation", entity.Operation);

                // Result — nullable (not present for COMPARE / DIVIDE)
                if (entity.ResultValue.HasValue)
                {
                    cmd.Parameters.AddWithValue("@resVal",  entity.ResultValue.Value);
                    cmd.Parameters.AddWithValue("@resUnit", entity.ResultUnit   ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@resType", entity.ResultMeasurementType ?? (object)DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@resVal",  DBNull.Value);
                    cmd.Parameters.AddWithValue("@resUnit", DBNull.Value);
                    cmd.Parameters.AddWithValue("@resType", DBNull.Value);
                }

                // Comparison result — only for COMPARE
                if (entity.Operation == OperationType.COMPARE.ToString())
                    cmd.Parameters.AddWithValue("@compResult", entity.ComparisonResult ? 1 : 0);
                else
                    cmd.Parameters.AddWithValue("@compResult", DBNull.Value);

                // Scalar result — only for DIVIDE
                if (entity.Operation == OperationType.DIVIDE.ToString())
                    cmd.Parameters.AddWithValue("@scalarResult", entity.ScalarResult);
                else
                    cmd.Parameters.AddWithValue("@scalarResult", DBNull.Value);

                // Error info
                cmd.Parameters.AddWithValue("@hasError", entity.HasError ? 1 : 0);
                cmd.Parameters.AddWithValue("@errorMessage", entity.HasError
                    ? (object)entity.ErrorMessage!
                    : DBNull.Value);

                // Timestamp
                cmd.Parameters.AddWithValue("@timestamp", entity.Timestamp);

                cmd.ExecuteNonQuery();

                Console.WriteLine("[DatabaseRepository] Saved: " + entity.Operation
                    + " at " + entity.Timestamp);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Failed to save entity: " + ex.Message, ex);
            }
            finally
            {
                _pool.ReleaseConnection(conn);
            }
        }

        // ── GET ALL ───────────────────────────────────────────

        public IList<QuantityMeasurementEntity> GetAll()
        {
            SqlConnection conn = _pool.AcquireConnection();

            try
            {
                string sql = "SELECT * FROM quantity_measurement_entity ORDER BY timestamp DESC";

                using SqlCommand    cmd    = new SqlCommand(sql, conn);
                using SqlDataReader reader = cmd.ExecuteReader();

                return MapRows(reader);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Failed to get all entities: " + ex.Message, ex);
            }
            finally
            {
                _pool.ReleaseConnection(conn);
            }
        }

        // ── GET BY OPERATION ──────────────────────────────────

        public IList<QuantityMeasurementEntity> GetByOperation(string operation)
        {
            if (string.IsNullOrWhiteSpace(operation))
                throw new ArgumentException("Operation cannot be empty.");

            SqlConnection conn = _pool.AcquireConnection();

            try
            {
                string sql = @"
                    SELECT * FROM quantity_measurement_entity
                    WHERE operation = @operation
                    ORDER BY timestamp DESC";

                using SqlCommand    cmd    = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@operation", operation.ToUpper());

                using SqlDataReader reader = cmd.ExecuteReader();
                return MapRows(reader);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Failed to get by operation: " + ex.Message, ex);
            }
            finally
            {
                _pool.ReleaseConnection(conn);
            }
        }

        // ── GET BY MEASUREMENT TYPE ───────────────────────────

        public IList<QuantityMeasurementEntity> GetByMeasurementType(string measurementType)
        {
            if (string.IsNullOrWhiteSpace(measurementType))
                throw new ArgumentException("Measurement type cannot be empty.");

            SqlConnection conn = _pool.AcquireConnection();

            try
            {
                string sql = @"
                    SELECT * FROM quantity_measurement_entity
                    WHERE operand1_type = @measurementType
                    ORDER BY timestamp DESC";

                using SqlCommand    cmd    = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@measurementType", measurementType.ToUpper());

                using SqlDataReader reader = cmd.ExecuteReader();
                return MapRows(reader);
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Failed to get by type: " + ex.Message, ex);
            }
            finally
            {
                _pool.ReleaseConnection(conn);
            }
        }

        // ── GET TOTAL COUNT ───────────────────────────────────

        public int GetTotalCount()
        {
            SqlConnection conn = _pool.AcquireConnection();

            try
            {
                string sql = "SELECT COUNT(*) FROM quantity_measurement_entity";

                using SqlCommand cmd = new SqlCommand(sql, conn);
                return (int)cmd.ExecuteScalar();
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Failed to get count: " + ex.Message, ex);
            }
            finally
            {
                _pool.ReleaseConnection(conn);
            }
        }

        // ── DELETE ALL ────────────────────────────────────────

        public void Clear()
        {
            SqlConnection conn = _pool.AcquireConnection();

            try
            {
                string sql = "DELETE FROM quantity_measurement_entity";

                using SqlCommand cmd  = new SqlCommand(sql, conn);
                int              rows = cmd.ExecuteNonQuery();

                Console.WriteLine("[DatabaseRepository] Deleted " + rows + " records.");
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Failed to delete all: " + ex.Message, ex);
            }
            finally
            {
                _pool.ReleaseConnection(conn);
            }
        }

        // ── POOL STATISTICS ───────────────────────────────────

        public string GetPoolStatistics()
        {
            return _pool.GetPoolStatistics();
        }

        // ── RELEASE RESOURCES ─────────────────────────────────

        public void ReleaseResources()
        {
            _pool.Dispose();
            Console.WriteLine("[DatabaseRepository] Resources released.");
        }

        // ── PRIVATE — MAP SQL ROWS TO ENTITIES ───────────────

        private IList<QuantityMeasurementEntity> MapRows(SqlDataReader reader)
        {
            List<QuantityMeasurementEntity> list = new List<QuantityMeasurementEntity>();

            while (reader.Read())
            {
                QuantityMeasurementEntity entity = new QuantityMeasurementEntity();

                entity.Operand1Value           = reader.GetDouble(reader.GetOrdinal("operand1_value"));
                entity.Operand1Unit            = reader.GetString(reader.GetOrdinal("operand1_unit"));
                entity.Operand1MeasurementType = reader.GetString(reader.GetOrdinal("operand1_type"));

                if (!reader.IsDBNull(reader.GetOrdinal("operand2_value")))
                {
                    entity.Operand2Value           = reader.GetDouble(reader.GetOrdinal("operand2_value"));
                    entity.Operand2Unit            = reader.GetString(reader.GetOrdinal("operand2_unit"));
                    entity.Operand2MeasurementType = reader.GetString(reader.GetOrdinal("operand2_type"));
                }

                entity.Operation = reader.GetString(reader.GetOrdinal("operation"));

                if (!reader.IsDBNull(reader.GetOrdinal("result_value")))
                {
                    entity.ResultValue           = reader.GetDouble(reader.GetOrdinal("result_value"));
                    entity.ResultUnit            = reader.GetString(reader.GetOrdinal("result_unit"));
                    entity.ResultMeasurementType = reader.GetString(reader.GetOrdinal("result_type"));
                }

                if (!reader.IsDBNull(reader.GetOrdinal("comparison_result")))
                    entity.ComparisonResult = reader.GetBoolean(reader.GetOrdinal("comparison_result"));

                if (!reader.IsDBNull(reader.GetOrdinal("scalar_result")))
                    entity.ScalarResult = reader.GetDouble(reader.GetOrdinal("scalar_result"));

                entity.HasError = reader.GetBoolean(reader.GetOrdinal("has_error"));

                if (!reader.IsDBNull(reader.GetOrdinal("error_message")))
                    entity.ErrorMessage = reader.GetString(reader.GetOrdinal("error_message"));

                entity.Timestamp = reader.GetDateTime(reader.GetOrdinal("timestamp"));

                list.Add(entity);
            }

            return list;
        }
    }
}