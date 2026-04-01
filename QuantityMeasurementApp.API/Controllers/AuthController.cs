using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuantityMeasurementApp.Business;
using QuantityMeasurementApp.Business.Exceptions;
using QuantityMeasurementApp.Business.Implementations;
using QuantityMeasurementApp.Entity.DTOs;

namespace QuantityMeasurementApp.API.Controllers
{
    [ApiController]
    [Route("api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService        _authService;
        private readonly AesEncryptionService _aesEncryption;

        public AuthController(
            IAuthService         authService,
            AesEncryptionService aesEncryption)
        {
            _authService   = authService;
            _aesEncryption = aesEncryption;
        }

        // ── REGISTER ─────────────────────────────────────────

        // POST api/v1/auth/register
        [HttpPost("register")]
        [AllowAnonymous]
        public IActionResult Register(RegisterDTO dto)
        {
            try
            {
                AuthResponseDTO result = _authService.Register(dto);
                return Ok(result);
            }
            catch (QuantityMeasurementException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // ── LOGIN ─────────────────────────────────────────────

        // POST api/v1/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login(LoginDTO dto)
        {
            try
            {
                AuthResponseDTO result = _authService.Login(dto);
                return Ok(result);
            }
            catch (QuantityMeasurementException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // ── ENCRYPT ──────────────────────────────────────────

        // POST api/v1/auth/encrypt
        [HttpPost("encrypt")]
        [AllowAnonymous]
        public IActionResult Encrypt([FromBody] EncryptRequestDTO request)
        {
            try
            {
                string encrypted = _aesEncryption.Encrypt(request.PlainText);
                return Ok(new
                {
                    PlainText  = request.PlainText,
                    Encrypted  = encrypted,
                    Algorithm  = "AES-256-CBC",
                    KeySize    = "256 bits",
                    Message    = "Encryption successful."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // ── DECRYPT ──────────────────────────────────────────

        // POST api/v1/auth/decrypt
        [HttpPost("decrypt")]
        [AllowAnonymous]
        public IActionResult Decrypt([FromBody] DecryptRequestDTO request)
        {
            try
            {
                string decrypted = _aesEncryption.Decrypt(request.CipherText);
                return Ok(new
                {
                    CipherText = request.CipherText,
                    Decrypted  = decrypted,
                    Algorithm  = "AES-256-CBC",
                    KeySize    = "256 bits",
                    Message    = "Decryption successful."
                });
            }
            catch (FormatException)
            {
                return BadRequest(new
                {
                    Message = "Invalid Base64 cipher text. " +
                              "Make sure you paste the exact encrypted string."
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}