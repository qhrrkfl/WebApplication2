using DBconnect.Models;
using DBconnection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Security.Claims;
using System.Threading.Tasks;
using WebApplication2.Models;
using WebApplication2.Service;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;


namespace WebApplication2.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {

        private readonly ILogger<AccountController> _logger;
        private readonly TranslateDbContext _dbContext;
        private readonly AccountService _accountService;
        private readonly MailValService _mailValService;
        public AccountController(ILogger<AccountController> logger, TranslateDbContext _dbconnection, AccountService AC, MailValService mailValService)
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

            var user = await _accountService.ValidateLogin(vm.Email, vm.Password, ct);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "아이디 또는 비밀번호가 올바르지 않습니다.");
                return View(vm);
            }
            else
            {

                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name , user.Email),
                        new Claim("urn:myapp:nickname", user.NickName),    // 커스텀
                        new Claim("validated" , user.IsValidated ? "true" : "false"),
                        new Claim(ClaimTypes.Role , "USER")
                    };


                //// 읽기 위 접근 불가능한 밸류는 이렇게 읽음.
                //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //var email = User.FindFirstValue(ClaimTypes.Email);
                //var nickname = User.FindFirst("urn:myapp:nickname")?.Value;

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true,                     // 브라우저 닫아도 유지
                        ExpiresUtc = DateTime.UtcNow.AddHours(4) // 쿠키 만료 시간
                    });

                if (user.IsValidated)
                {
                    return RedirectToAction("Welcome", "Home");
                }
                else
                {
                    return RedirectToAction(nameof(Validation));
                    //PRG(Post-Redirect-Get) 패턴으로 한 번 리다이렉트해 “로그인 후 신원”으로 새 토큰을 발급받게 해야 합니다.
                }

                // validation page로 return
            }

        }

        [HttpGet]
        [Authorize]
        public IActionResult Validation() => View("Validation"); // Views/Account/Validation.cshtml


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
        public async Task<IActionResult> Register(RegisterVm vm, CancellationToken ct)
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

        [AcceptVerbs("POST")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CheckEmail(string Email, CancellationToken ct)
        {
            var ret = await _dbContext.Users.Where(x => x.Email.ToLower().Equals(Email)).AsNoTracking().FirstOrDefaultAsync(ct);
            bool exist = false;
            if (ret != null)
            {
                exist = true;
            }
            // Remote는 true면 유효, 문자열이면 에러메시지
            return Json(exist ? "이미 사용 중인 이메일입니다." : true);
        }

        [AcceptVerbs("POST")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CheckNickName(string Nickname, CancellationToken ct)
        {
            var ret = await _dbContext.Users.Where(x => x.NickName.ToLower().Equals(Nickname)).AsNoTracking().FirstOrDefaultAsync(ct);
            bool exist = false;
            if (ret != null)
            {
                exist = true;
            }
            // Remote는 true면 유효, 문자열이면 에러메시지
            return Json(exist ? "이미 사용 중인 닉네임 입니다." : true);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> SendValMail(CancellationToken ct)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var email = User.FindFirstValue(ClaimTypes.Name);
                //var nickname = User.FindFirst("urn:myapp:nickname")?.Value;

                var ret = await _mailValService.sendEmail(email, ct);
                if (ret.Equals("발송"))
                {
                    return Ok();

                }
                else
                {
                    return BadRequest(ret);
                }
            }

            return View("Login");

        }





        #region
        // 검증 완료시 claim을 수정하고 쿠키를 재발급 해야한다.
        //        // 이메일 코드 확인 성공 후:
        //        var claims = User.Claims.ToList();
        //        var idx = claims.FindIndex(c => c.Type == "validated");
        //if (idx >= 0) claims.RemoveAt(idx);
        //claims.Add(new Claim("validated", "true"));
        //
        //var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        //        var principal = new ClaimsPrincipal(identity);
        //
        //        // 쿠키 재발급
        //        await HttpContext.SignInAsync(
        //            CookieAuthenticationDefaults.AuthenticationScheme,
        //            principal,
        //    new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTime.UtcNow.AddHours(4) });
        //
        //// 이제 권한에 따라 사용 가능 (예: 환영 페이지)
        //return RedirectToAction("Welcome", "Home");
        #endregion




    }
}
