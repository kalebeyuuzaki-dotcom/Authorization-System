using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portifolio.Models.Domain
{
    [Table("RecoverPasswordrRequests")] 
    public class RecoverPasswordRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public Guid Id { get; set; }

        [Required]
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(6)]
        public string VerificationCode { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiresAt { get; set; }

        public int AccessFailedCount { get; set; } = 0;
    }
}
