using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class SurveyViewModel
    {
        [Display(Name = "항목 1")]
        [Required(ErrorMessage = "항목 1을 입력해 주세요.")]
        [StringLength(500)]
        public string Item1 { get; set; }

        [Display(Name = "항목 2")]
        [Required(ErrorMessage = "항목 2를 입력해 주세요.")]
        [StringLength(500)]
        public string Item2 { get; set; }

        [Display(Name = "항목 3")]
        [Required(ErrorMessage = "항목 3을 입력해 주세요.")]
        [StringLength(500)]
        public string Item3 { get; set; }
    }
}