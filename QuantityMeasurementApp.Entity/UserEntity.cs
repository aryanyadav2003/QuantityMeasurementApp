using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuantityMeasurementApp.Entity.Enums;

namespace QuantityMeasurementApp.Entity
{
    [Table("Users")]
    public class UserEntity
    {
        // ── Primary Key ───────────────────────────────────────

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // ── User Info ─────────────────────────────────────────

        [Required(ErrorMessage = "FullName is required.")]
        [Column("FullName")]
        [StringLength(100, ErrorMessage = "FullName must not exceed 100 characters.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [Column("Email")]
        [StringLength(100, ErrorMessage = "Email must not exceed 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        // BCrypt hashed then AES-256 encrypted before storing
        [Required(ErrorMessage = "PasswordHash is required.")]
        [Column("PasswordHash")]
        public string PasswordHash { get; set; }

        // ── Role ─────────────────────────────────────────────

        [Required]
        [Column("Role")]
        [StringLength(20)]
        public UserRole Role { get; set; }

        // ── Metadata ──────────────────────────────────────────

        [Required]
        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Required]
        [Column("IsActive")]
        public bool IsActive { get; set; }

        // Parameterless constructor — required by EF Core
        public UserEntity()
        {
            CreatedAt = DateTime.UtcNow;
            IsActive  = true;
            Role      = UserRole.USER;
        }

        public override string ToString()
        {
            return "[User] Id=" + Id
                + " | Name="   + FullName
                + " | Email="  + Email
                + " | Role="   + Role
                + " | Active=" + IsActive
                + " | Created=" + CreatedAt.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}