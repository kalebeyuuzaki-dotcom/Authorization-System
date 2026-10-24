using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portifolio.Models.Domain
{
    [Table("PreRegisterRequests")] 
    public class PreRegisterRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public Guid Id { get; set; }

        [Required]
        [MaxLength(150)] 
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(6)]
        public string VerificationCode { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiresAt { get; set; }

        public byte AccessFailedCount { get; set; } = 0;

        public bool IsVerified { get; set; } = false;
    }
}
