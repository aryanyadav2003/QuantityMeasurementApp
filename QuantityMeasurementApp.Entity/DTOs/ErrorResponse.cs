using System;
using System.ComponentModel.DataAnnotations;

namespace QuantityMeasurementApp.Entity.DTOs
{
    public class ErrorResponse
    {
        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        [Range(100, 599, ErrorMessage = "Status must be a valid HTTP status code.")]
        public int Status { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Error description must not exceed 100 characters.")]
        public string Error { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Message must not exceed 500 characters.")]
        public string Message { get; set; }

        [Required]
        [StringLength(200, ErrorMessage = "Path must not exceed 200 characters.")]
        public string Path { get; set; }
    }
}