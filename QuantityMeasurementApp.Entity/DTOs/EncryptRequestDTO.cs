using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementApp.Entity.DTOs
{
    public class EncryptRequestDTO
    {
        [Required(ErrorMessage = "PlainText is required.")]
        public string PlainText { get; set; }
    }
}