using DBconnect.Models;
using DBconnection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Threading.Tasks;
using WebApplication2.Models;
using WebApplication2.Service;

namespace WebApplication2.Controllers
{
    [AllowAnonymous]
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
        public async Task<IActionResult> Register(RegisterVm vm , CancellationToken ct)
        {

            var ret = await _accountService.RegisterID(vm.Email, vm.rawPassword, vm.Nickname, ct);
            if (ret == false)
            {

                ModelState.AddModelError(string.Empty, "회원가입에 실패했습니다. 잠시 후 다시 시도해 주세요.");
                return View(vm);
            }

            return RedirectToAction("Index", "Translation");
        }

        [HttpGet]
        public IActionResult Denied() { return View(); }

        [AcceptVerbs( "POST")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CheckEmail(string Email, CancellationToken ct)
        {
            var ret  = await _dbContext.Users.Where(x => x.Email.ToLower().Equals(Email)).AsNoTracking().FirstOrDefaultAsync(ct);
            bool exist = false;
            if(ret != null)
            {
                exist = true;
            }
            // Remote는 true면 유효, 문자열이면 에러메시지
            return Json(exist ? "이미 사용 중인 이메일입니다." : true);
        }

        [AcceptVerbs( "POST")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CheckNickName(string Nickname, CancellationToken ct)
        {
            var ret = await _dbContext.Users.Where(x => x.NickName.ToLower().Equals(Nickname)).AsNoTracking().FirstOrDefaultAsync( ct);
            bool exist = false;
            if (ret != null)
            {
                exist = true;
            }
            // Remote는 true면 유효, 문자열이면 에러메시지
            return Json(exist ? "이미 사용 중인 닉네임 입니다." : true);
        }

        





    }
}
