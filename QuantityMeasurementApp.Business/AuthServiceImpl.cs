using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using QuantityMeasurementApp.Business.Exceptions;
using QuantityMeasurementApp.Business.Implementations;
using QuantityMeasurementApp.Entity;
using QuantityMeasurementApp.Entity.DTOs;
using QuantityMeasurementApp.Entity.Enums;
using QuantityMeasurementApp.Interface.Repository;

namespace QuantityMeasurementApp.Business
{
    public class AuthServiceImpl : IAuthService
    {
        private readonly IUserRepository      _userRepository;
        private readonly IConfiguration       _configuration;
        private readonly AesEncryptionService _aesEncryption;

        public AuthServiceImpl(
            IUserRepository      userRepository,
            IConfiguration       configuration,
            AesEncryptionService aesEncryption)
        {
            _userRepository = userRepository;
            _configuration  = configuration;
            _aesEncryption  = aesEncryption;
        }

        // ── REGISTER ─────────────────────────────────────────

        public AuthResponseDTO Register(RegisterDTO dto)
        {
            // Check duplicate email
            if (_userRepository.EmailExists(dto.Email))
                throw new QuantityMeasurementException(
                    "Email already registered: " + dto.Email);

            // Parse role
            UserRole role = UserRole.USER;
            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                if (Enum.TryParse<UserRole>(
                    dto.Role.ToUpper(), ignoreCase: true, out UserRole parsedRole))
                    role = parsedRole;
            }

            // Step 1 — Hash password using BCrypt
            string bcryptHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // Step 2 — Encrypt the BCrypt hash using AES-256
            string encryptedPasswordHash = _aesEncryption.Encrypt(bcryptHash);

            Console.WriteLine("[AuthService] BCrypt Hash    : " + bcryptHash);
            Console.WriteLine("[AuthService] Encrypted Hash : " + encryptedPasswordHash);

            // Store AES-encrypted BCrypt hash in DB
            UserEntity user = new UserEntity
            {
                FullName     = dto.FullName,
                Email        = dto.Email.ToLower(),
                PasswordHash = encryptedPasswordHash,
                Role         = role
            };

            _userRepository.Save(user);

            string token = GenerateJwtToken(user);

            return new AuthResponseDTO
            {
                Token     = token,
                Email     = user.Email,
                FullName  = user.FullName,
                Role      = user.Role.ToString(),
                Message   = "Registration successful.",
                IsSuccess = true
            };
        }

        // ── LOGIN ─────────────────────────────────────────────

        public AuthResponseDTO Login(LoginDTO dto)
        {
            UserEntity? user = _userRepository.GetByEmail(dto.Email);

            if (user == null)
                throw new QuantityMeasurementException(
                    "Invalid email or password.");

            if (!user.IsActive)
                throw new QuantityMeasurementException(
                    "Account is deactivated. Contact admin.");

            // Step 1 — Decrypt the stored AES-encrypted BCrypt hash
            string decryptedBcryptHash = _aesEncryption.Decrypt(user.PasswordHash);

            Console.WriteLine("[AuthService] Decrypted Hash : " + decryptedBcryptHash);

            // Step 2 — Verify input password against decrypted BCrypt hash
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(
                dto.Password, decryptedBcryptHash);

            if (!isPasswordValid)
                throw new QuantityMeasurementException(
                    "Invalid email or password.");

            string token = GenerateJwtToken(user);

            return new AuthResponseDTO
            {
                Token     = token,
                Email     = user.Email,
                FullName  = user.FullName,
                Role      = user.Role.ToString(),
                Message   = "Login successful.",
                IsSuccess = true
            };
        }

        // ── PRIVATE — JWT Token Generator ─────────────────────

        private string GenerateJwtToken(UserEntity user)
        {
            string jwtKey    = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key not configured.");
            string jwtIssuer = _configuration["Jwt:Issuer"]   ?? "QuantityMeasurementApp";
            string jwtAud    = _configuration["Jwt:Audience"] ?? "QuantityMeasurementApp";
            int    expMins   = int.Parse(
                _configuration["Jwt:ExpiryMinutes"] ?? "60");

            SymmetricSecurityKey key   =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            SigningCredentials   creds =
                new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            Claim[] claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,   user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Name,               user.FullName),
                new Claim(ClaimTypes.Role,               user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer:             jwtIssuer,
                audience:           jwtAud,
                claims:             claims,
                expires:            DateTime.UtcNow.AddMinutes(expMins),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}