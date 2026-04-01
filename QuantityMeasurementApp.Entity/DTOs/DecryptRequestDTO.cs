using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementApp.Entity.DTOs
{
    public class DecryptRequestDTO
    {
        [Required(ErrorMessage = "CipherText is required.")]
        public string CipherText { get; set; }
    }
}