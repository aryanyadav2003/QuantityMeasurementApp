using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementApp.Entity.DTOs
{
    public class QuantityMeasurementDTO
    {
        // Input operand 1
        public double ThisValue           { get; set; }

        [StringLength(50)]
        public string ThisUnit            { get; set; }

        [StringLength(50)]
        public string ThisMeasurementType { get; set; }

        // Input operand 2 — null for Convert
        public double? ThatValue           { get; set; }

        [StringLength(50)]
        public string  ThatUnit            { get; set; }

        [StringLength(50)]
        public string  ThatMeasurementType { get; set; }

        // Operation performed e.g. "COMPARE", "CONVERT", "ADD"
        [Required]
        [StringLength(20)]
        public string Operation { get; set; }

        // Result for Compare operation — "True" or "False"
        public string ResultString { get; set; }

        // Result for Convert, Add, Subtract operations
        public double? ResultValue           { get; set; }

        [StringLength(50)]
        public string  ResultUnit            { get; set; }

        [StringLength(50)]
        public string  ResultMeasurementType { get; set; }

        // Scalar result for Divide operation
        public double? ScalarResult { get; set; }

        // Error info
        public bool   IsError      { get; set; }

        [StringLength(500)]
        public string ErrorMessage { get; set; }

        // ── Static factory methods ────────────────────────────

        public static QuantityMeasurementDTO FromCompare(
            QuantityDTO q1, QuantityDTO q2, bool result)
        {
            QuantityMeasurementDTO dto = new QuantityMeasurementDTO();
            dto.ThisValue           = q1.Value;
            dto.ThisUnit            = q1.Unit;
            dto.ThisMeasurementType = q1.MeasurementType;
            dto.ThatValue           = q2.Value;
            dto.ThatUnit            = q2.Unit;
            dto.ThatMeasurementType = q2.MeasurementType;
            dto.Operation           = "COMPARE";
            dto.ResultString        = result.ToString();
            dto.IsError             = false;
            return dto;
        }

        public static QuantityMeasurementDTO FromConvert(
            QuantityDTO input, QuantityDTO result)
        {
            QuantityMeasurementDTO dto = new QuantityMeasurementDTO();
            dto.ThisValue             = input.Value;
            dto.ThisUnit              = input.Unit;
            dto.ThisMeasurementType   = input.MeasurementType;
            dto.Operation             = "CONVERT";
            dto.ResultValue           = result.Value;
            dto.ResultUnit            = result.Unit;
            dto.ResultMeasurementType = result.MeasurementType;
            dto.IsError               = false;
            return dto;
        }

        public static QuantityMeasurementDTO FromArithmetic(
            QuantityDTO q1, QuantityDTO q2, QuantityDTO result, string operation)
        {
            QuantityMeasurementDTO dto = new QuantityMeasurementDTO();
            dto.ThisValue             = q1.Value;
            dto.ThisUnit              = q1.Unit;
            dto.ThisMeasurementType   = q1.MeasurementType;
            dto.ThatValue             = q2.Value;
            dto.ThatUnit              = q2.Unit;
            dto.ThatMeasurementType   = q2.MeasurementType;
            dto.Operation             = operation;
            dto.ResultValue           = result.Value;
            dto.ResultUnit            = result.Unit;
            dto.ResultMeasurementType = result.MeasurementType;
            dto.IsError               = false;
            return dto;
        }

        public static QuantityMeasurementDTO FromDivide(
            QuantityDTO q1, QuantityDTO q2, double scalar)
        {
            QuantityMeasurementDTO dto = new QuantityMeasurementDTO();
            dto.ThisValue           = q1.Value;
            dto.ThisUnit            = q1.Unit;
            dto.ThisMeasurementType = q1.MeasurementType;
            dto.ThatValue           = q2.Value;
            dto.ThatUnit            = q2.Unit;
            dto.ThatMeasurementType = q2.MeasurementType;
            dto.Operation           = "DIVIDE";
            dto.ScalarResult        = scalar;
            dto.IsError             = false;
            return dto;
        }

        public static QuantityMeasurementDTO FromError(
            string operation, string errorMessage)
        {
            QuantityMeasurementDTO dto = new QuantityMeasurementDTO();
            dto.Operation    = operation;
            dto.IsError      = true;
            dto.ErrorMessage = errorMessage;
            return dto;
        }
    }
}