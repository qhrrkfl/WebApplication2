namespace WebApplication2.Models
{

    using System.ComponentModel.DataAnnotations;

    public class LoginVm
    {
        [Required(ErrorMessage = "이메일을 입력해 주세요.")]
        [EmailAddress(ErrorMessage = "이메일 형식이 아닙니다.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "비밀번호를 입력해 주세요.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "비밀번호는 최소 8자 입니다.")]
        public string Password { get; set; } = "";
    }


    public class RegisterVm 
    {
        [Required(ErrorMessage ="이메일을 입력해 주세요")]
        [EmailAddress(ErrorMessage ="이메일 형식이 아닙니다.")]
        public string Email { get; set; }

        public bool? EmailCheck { get; set; } = null;
        [Required(ErrorMessage = "닉네임을 입력해 주세요")]
        public string Nickname { get; set; }
        public bool? nicknameCheck { get; set; } = null;

        [Required(ErrorMessage = "비밀번호는 최소 8자 입니다.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "비밀번호는 최소 8자입니다.")]
        public string rawPassword { get; set; }

        public string Code { get; set; }
        public bool? CodeCheck { get; set; } = null;
    }


}
