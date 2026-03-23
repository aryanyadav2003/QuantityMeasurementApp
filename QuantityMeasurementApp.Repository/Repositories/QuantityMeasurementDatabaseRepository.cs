using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Repository.Utilities;
using QuantityMeasurementApp.Repository.Exceptions;


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
                cmd.Parameters.AddWithValue("@op1Val", entity.Operand1.Value);
                cmd.Parameters.AddWithValue("@op1Unit", entity.Operand1.Unit);
                cmd.Parameters.AddWithValue("@op1Type", entity.Operand1.MeasurementType);

                // Operand 2 — nullable (not present for CONVERT)
                if (entity.Operand2 != null)
                {
                    cmd.Parameters.AddWithValue("@op2Val", entity.Operand2.Value);
                    cmd.Parameters.AddWithValue("@op2Unit", entity.Operand2.Unit);
                    cmd.Parameters.AddWithValue("@op2Type", entity.Operand2.MeasurementType);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@op2Val", DBNull.Value);
                    cmd.Parameters.AddWithValue("@op2Unit", DBNull.Value);
                    cmd.Parameters.AddWithValue("@op2Type", DBNull.Value);
                }

                // Operation type
                cmd.Parameters.AddWithValue("@operation", entity.Operation.ToString());

                // Result — nullable (not present for COMPARE/DIVIDE)
                if (entity.Result != null)
                {
                    cmd.Parameters.AddWithValue("@resVal", entity.Result.Value);
                    cmd.Parameters.AddWithValue("@resUnit", entity.Result.Unit);
                    cmd.Parameters.AddWithValue("@resType", entity.Result.MeasurementType);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@resVal", DBNull.Value);
                    cmd.Parameters.AddWithValue("@resUnit", DBNull.Value);
                    cmd.Parameters.AddWithValue("@resType", DBNull.Value);
                }

                // Comparison result — only for COMPARE operation
                if (entity.Operation == OperationType.COMPARE)
                    cmd.Parameters.AddWithValue("@compResult", entity.ComparisonResult ? 1 : 0);
                else
                    cmd.Parameters.AddWithValue("@compResult", DBNull.Value);

                // Scalar result — only for DIVIDE operation
                if (entity.Operation == OperationType.DIVIDE)
                    cmd.Parameters.AddWithValue("@scalarResult", entity.ScalarResult);
                else
                    cmd.Parameters.AddWithValue("@scalarResult", DBNull.Value);

                // Error info
                cmd.Parameters.AddWithValue("@hasError", entity.HasError ? 1 : 0);
                cmd.Parameters.AddWithValue("@errorMessage", entity.HasError
                    ? (object)entity.ErrorMessage
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

                using SqlCommand cmd = new SqlCommand(sql, conn);
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

                using SqlCommand cmd = new SqlCommand(sql, conn);
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

                using SqlCommand cmd = new SqlCommand(sql, conn);
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

                using SqlCommand cmd = new SqlCommand(sql, conn);
                int rows = cmd.ExecuteNonQuery();

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
                // Read operand 1 — always present
                QuantityDTO operand1 = new QuantityDTO(
                    reader.GetDouble(reader.GetOrdinal("operand1_value")),
                    reader.GetString(reader.GetOrdinal("operand1_unit")),
                    reader.GetString(reader.GetOrdinal("operand1_type")));

                // Read operand 2 — nullable
                QuantityDTO? operand2 = null;
                if (!reader.IsDBNull(reader.GetOrdinal("operand2_value")))
                {
                    operand2 = new QuantityDTO(
                        reader.GetDouble(reader.GetOrdinal("operand2_value")),
                        reader.GetString(reader.GetOrdinal("operand2_unit")),
                        reader.GetString(reader.GetOrdinal("operand2_type")));
                }

                // Read operation type
                string opString = reader.GetString(reader.GetOrdinal("operation"));
                OperationType operation = Enum.Parse<OperationType>(opString);

                // Read result — nullable
                QuantityDTO? result = null;
                if (!reader.IsDBNull(reader.GetOrdinal("result_value")))
                {
                    result = new QuantityDTO(
                        reader.GetDouble(reader.GetOrdinal("result_value")),
                        reader.GetString(reader.GetOrdinal("result_unit")),
                        reader.GetString(reader.GetOrdinal("result_type")));
                }

                // Read error info
                bool hasError = reader.GetBoolean(reader.GetOrdinal("has_error"));
                string? errorMessage = null;
                if (!reader.IsDBNull(reader.GetOrdinal("error_message")))
                    errorMessage = reader.GetString(reader.GetOrdinal("error_message"));

                // Build entity based on operation type
                QuantityMeasurementEntity entity;

                if (hasError)
                {
                    entity = new QuantityMeasurementEntity(
                        operand1, operand2, operation, errorMessage ?? "Unknown error");
                }
                else if (operation == OperationType.COMPARE)
                {
                    bool compResult = !reader.IsDBNull(reader.GetOrdinal("comparison_result"))
                        && reader.GetBoolean(reader.GetOrdinal("comparison_result"));

                    entity = new QuantityMeasurementEntity(
                        operand1, operand2, operation, compResult);
                }
                else if (operation == OperationType.DIVIDE)
                {
                    double scalar = reader.IsDBNull(reader.GetOrdinal("scalar_result"))
                        ? 0
                        : reader.GetDouble(reader.GetOrdinal("scalar_result"));

                    entity = new QuantityMeasurementEntity(
                        operand1, operand2, operation, scalar);
                }
                else if (operation == OperationType.CONVERT)
                {
                    entity = new QuantityMeasurementEntity(
                        operand1, operation, result!);
                }
                else
                {
                    // ADD or SUBTRACT
                    entity = new QuantityMeasurementEntity(
                        operand1, operand2!, operation, result!);
                }

                list.Add(entity);
            }

            return list;
        }
    }
}