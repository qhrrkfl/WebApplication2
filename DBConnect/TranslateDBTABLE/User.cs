using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBconnect.Models
{
    [Table("Users")]
    public class User
    {
        // 이메일을 Primary Key로 사용
        [Key]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? NickName { get; set; }
        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string? HashPassword { get; set; }

        [MaxLength(50)]
        public string? Provider { get; set; } = string.Empty;

        // OAuth 공급자가 제공하는 고유 사용자 ID
        [MaxLength(255)]
        public string? ProviderId { get; set; } = string.Empty;

        // 계정 생성 시각
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // 마지막 로그인 시각
        public DateTime? LastLogin { get; set; } = DateTime.Now;

        public bool IsValidated { get; set; }
    }
}