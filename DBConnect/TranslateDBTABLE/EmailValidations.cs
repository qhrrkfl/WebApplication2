using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBConnect.TranslateDBTABLE
{
    [Table("EmailValidations")]
    [PrimaryKey(nameof(CreatedAt) , nameof(Email))]
    public class EmailValidations
    {
        [Column(Order =1)]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [MaxLength(32)]
        public byte[] ValCode { get; set; }
        // 이 발리데이션 코드가 사용 가능한지 (사용한건지, 다른 코드를 발급 받은건지)
        public bool IsValid { get; set; }

        [Column(Order = 0)]
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
