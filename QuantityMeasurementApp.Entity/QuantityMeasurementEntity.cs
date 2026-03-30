using System;
using QuantityMeasurementApp.Entity.Enums;
using QuantityMeasurementApp.Entity.DTOs;

namespace QuantityMeasurementApp.Entity
{
    public class QuantityMeasurementEntity
    {
        // Primary key for EF Core
        public int Id { get; set; }

        // Operand 1 — flat columns
        public double  Operand1Value           { get; set; }
        public string  Operand1Unit            { get; set; }
        public string  Operand1MeasurementType { get; set; }

        // Operand 2 — nullable flat columns
        public double? Operand2Value           { get; set; }
        public string? Operand2Unit            { get; set; }
        public string? Operand2MeasurementType { get; set; }

        // Operation stored as string
        public string Operation { get; set; }

        // Result — nullable flat columns
        public double? ResultValue           { get; set; }
        public string? ResultUnit            { get; set; }
        public string? ResultMeasurementType { get; set; }

        // Compare result
        public bool ComparisonResult { get; set; }

        // Divide result
        public double ScalarResult { get; set; }

        // Error info
        public bool    HasError     { get; set; }
        public string? ErrorMessage { get; set; }

        // Timestamp
        public DateTime Timestamp { get; set; }

        // Parameterless constructor — required by EF Core
        public QuantityMeasurementEntity()
        {
            Timestamp = DateTime.Now;
        }

        // ── Static factory methods ────────────────────────────

        // CONVERT
        public static QuantityMeasurementEntity ForConvert(
            QuantityDTO operand1, OperationType operation, QuantityDTO result)
        {
            QuantityMeasurementEntity e = new QuantityMeasurementEntity();
            e.Operand1Value           = operand1.Value;
            e.Operand1Unit            = operand1.Unit;
            e.Operand1MeasurementType = operand1.MeasurementType;
            e.Operation               = operation.ToString();
            e.ResultValue             = result.Value;
            e.ResultUnit              = result.Unit;
            e.ResultMeasurementType   = result.MeasurementType;
            e.HasError                = false;
            return e;
        }

        // ADD / SUBTRACT
        public static QuantityMeasurementEntity ForArithmetic(
            QuantityDTO operand1, QuantityDTO operand2,
            OperationType operation, QuantityDTO result)
        {
            QuantityMeasurementEntity e = new QuantityMeasurementEntity();
            e.Operand1Value           = operand1.Value;
            e.Operand1Unit            = operand1.Unit;
            e.Operand1MeasurementType = operand1.MeasurementType;
            e.Operand2Value           = operand2.Value;
            e.Operand2Unit            = operand2.Unit;
            e.Operand2MeasurementType = operand2.MeasurementType;
            e.Operation               = operation.ToString();
            e.ResultValue             = result.Value;
            e.ResultUnit              = result.Unit;
            e.ResultMeasurementType   = result.MeasurementType;
            e.HasError                = false;
            return e;
        }

        // COMPARE
        public static QuantityMeasurementEntity ForCompare(
            QuantityDTO operand1, QuantityDTO operand2,
            OperationType operation, bool comparisonResult)
        {
            QuantityMeasurementEntity e = new QuantityMeasurementEntity();
            e.Operand1Value           = operand1.Value;
            e.Operand1Unit            = operand1.Unit;
            e.Operand1MeasurementType = operand1.MeasurementType;
            e.Operand2Value           = operand2.Value;
            e.Operand2Unit            = operand2.Unit;
            e.Operand2MeasurementType = operand2.MeasurementType;
            e.Operation               = operation.ToString();
            e.ComparisonResult        = comparisonResult;
            e.HasError                = false;
            return e;
        }

        // DIVIDE
        public static QuantityMeasurementEntity ForDivide(
            QuantityDTO operand1, QuantityDTO operand2,
            OperationType operation, double scalarResult)
        {
            QuantityMeasurementEntity e = new QuantityMeasurementEntity();
            e.Operand1Value           = operand1.Value;
            e.Operand1Unit            = operand1.Unit;
            e.Operand1MeasurementType = operand1.MeasurementType;
            e.Operand2Value           = operand2.Value;
            e.Operand2Unit            = operand2.Unit;
            e.Operand2MeasurementType = operand2.MeasurementType;
            e.Operation               = operation.ToString();
            e.ScalarResult            = scalarResult;
            e.HasError                = false;
            return e;
        }

        // ERROR
        public static QuantityMeasurementEntity ForError(
            QuantityDTO operand1, QuantityDTO operand2,
            OperationType operation, string errorMessage)
        {
            QuantityMeasurementEntity e = new QuantityMeasurementEntity();
            e.Operand1Value           = operand1.Value;
            e.Operand1Unit            = operand1.Unit;
            e.Operand1MeasurementType = operand1.MeasurementType;
            if (operand2 != null)
            {
                e.Operand2Value           = operand2.Value;
                e.Operand2Unit            = operand2.Unit;
                e.Operand2MeasurementType = operand2.MeasurementType;
            }
            e.Operation    = operation.ToString();
            e.HasError     = true;
            e.ErrorMessage = errorMessage;
            return e;
        }

        public override string ToString()
        {
            string time = Timestamp.ToString("HH:mm:ss");

            if (HasError)
                return "[" + time + "] ERROR in " + Operation + ": " + ErrorMessage;

            if (Operation == OperationType.COMPARE.ToString())
                return "[" + time + "] " + Operation + ": "
                    + Operand1Value + " " + Operand1Unit
                    + " == "
                    + Operand2Value + " " + Operand2Unit
                    + " => " + ComparisonResult;

            if (Operation == OperationType.DIVIDE.ToString())
                return "[" + time + "] " + Operation + ": "
                    + Operand1Value + " " + Operand1Unit
                    + " / "
                    + Operand2Value + " " + Operand2Unit
                    + " = " + ScalarResult;

            if (Operand2Value != null)
                return "[" + time + "] " + Operation + ": "
                    + Operand1Value + " " + Operand1Unit
                    + " and "
                    + Operand2Value + " " + Operand2Unit
                    + " = " + ResultValue + " " + ResultUnit;

            return "[" + time + "] " + Operation + ": "
                + Operand1Value + " " + Operand1Unit
                + " => " + ResultValue + " " + ResultUnit;
        }
    }
}