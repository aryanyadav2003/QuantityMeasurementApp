using QuantityMeasurementApp.Entity.DTOs;

namespace QuantityMeasurementApp.Business
{
    public interface IAuthService
    {
        AuthResponseDTO Register(RegisterDTO dto);
        AuthResponseDTO Login(LoginDTO dto);
    }
}