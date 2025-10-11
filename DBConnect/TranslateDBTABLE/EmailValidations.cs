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
    public class EmailValidations
    {
        [Key]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }
        public int ValCode { get; set; }
        // 이 발리데이션 코드가 사용 가능한지 (사용한건지, 다른 코드를 발급 받은건지)
        public bool IsValid { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
