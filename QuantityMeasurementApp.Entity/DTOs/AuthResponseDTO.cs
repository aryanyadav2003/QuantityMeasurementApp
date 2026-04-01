namespace QuantityMeasurementApp.Entity.DTOs
{
    public class AuthResponseDTO
    {
        public string Token     { get; set; }
        public string Email     { get; set; }
        public string FullName  { get; set; }
        public string Role      { get; set; }
        public string Message   { get; set; }
        public bool   IsSuccess { get; set; }
    }
}