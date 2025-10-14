namespace WebApplication2.Models
{
    using Microsoft.AspNetCore.Mvc;
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
        //[EmailAddress(ErrorMessage ="이메일 형식이 아닙니다.")]
        [Remote(action: "CheckEmail" , controller: "Account", ErrorMessage ="이미 사용중인 이메일 입니다.", HttpMethod = "POST", AdditionalFields = "__RequestVerificationToken") ]
        public string Email { get; set; }


        [Required(ErrorMessage = "닉네임을 입력해 주세요")]
        [Remote(action: "CheckNickName" , controller:"Account" , ErrorMessage ="이미 사용중인 닉네임 입니다.", HttpMethod = "POST", AdditionalFields = "__RequestVerificationToken")]
        [MaxLength(10 , ErrorMessage ="닉네임은 최대 10자 입니다.")]
        public string Nickname { get; set; }

        [Required(ErrorMessage = "비밀번호는 최소 8자 입니다.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "비밀번호는 최소 8자입니다.")]
        public string rawPassword { get; set; }
    }


}
