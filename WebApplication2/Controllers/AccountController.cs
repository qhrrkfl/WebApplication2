using DBconnect.Models;
using DBconnection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApplication2.Models;
using WebApplication2.Service;

namespace WebApplication2.Controllers
{

    public class AccountController : Controller
    {

        private readonly ILogger<AccountController> _logger;
        private readonly TranslateDbContext _dbContext;
        private readonly AccountService _accountService;
        private readonly MailValService _mailValService;
        public AccountController(ILogger<AccountController> logger, TranslateDbContext _dbconnection , AccountService AC , MailValService mailValService)
        {
            _logger = logger;
            _dbContext = _dbconnection;
            _accountService = AC;
            _mailValService = mailValService;
        }


        // 로그인 페이지 표시 (GET)
        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Login() 
        { 
           return View(new LoginVm()); 
        }

        // 로그인 처리 (POST) ← 폼이 전송되는 곳
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Login(LoginVm vm, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(vm);

            var ok = await _accountService.ValidateLogin(vm.Email, vm.Password , ct);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, "아이디 또는 비밀번호가 올바르지 않습니다.");
                return View(vm);
            }

            // TODO: 쿠키 발급/클레임 생성 등
            return RedirectToAction("Welcome", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Register()
        {
            return View(new RegisterVm());
        }

        // 회원가입 폼 → 사용자 초기 등록 후 인증 메일 요청 페이지로 안내
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string command , RegisterVm vm , CancellationToken ct)
        {

            switch (command)
            {
                case "CheckEmail":
                    return View(vm);
                    break;

                case "CheckNickName":
                    return View(vm);
                    break;

                case "CodeSend":
                    await _mailValService.sendEmail(vm.Email, ct);
                    return View(vm);
                    break;
                default:
                    break;
            }



            return View(vm); 
            
        }

        [HttpGet]
        public IActionResult Denied() { return View(); }



    }
}
