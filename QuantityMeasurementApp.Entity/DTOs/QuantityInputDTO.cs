using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementApp.Entity.DTOs
{
    public class QuantityInputDTO
    {
        [Required(ErrorMessage = "ThisQuantity is required.")]
        public QuantityDTO ThisQuantity { get; set; }

        public QuantityDTO? ThatQuantity { get; set; }

        [StringLength(50, ErrorMessage = "TargetUnit must not exceed 50 characters.")]
        public string? TargetUnit { get; set; }
    }
}