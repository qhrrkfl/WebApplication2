using DBconnect.Models;
using DBconnection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApplication2.Controllers
{
    public class TranslationController : Controller
    {

        private readonly ILogger<TranslationController> _logger;
        private readonly TranslateDbContext _dbContext;

        public TranslationController(ILogger<TranslationController> logger, DBconnection.TranslateDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }


        #region oauth 도메인 없어서 못씀 ㅋ
        //    // 1) 구글 로그인 시작
        //    [HttpGet("login/google")]
        //    public IActionResult LoginWithGoogle()
        //    {
        //        // 로그인 성공 후 최종적으로 돌아올 우리 쪽 URL
        //        var redirectUrl = Url.Action("GoogleCallback", "Translation");
        //        var props = new AuthenticationProperties { RedirectUri = redirectUrl };
        //        return Challenge(props, GoogleDefaults.AuthenticationScheme);
        //    }

        //    // 2) 구글 로그인 완료 후 후처리(우리 앱 내부 이동 지점)
        //    //    콜백(/signin-google)은 미들웨어가 처리하고,
        //    //    그 다음 여기(GoogleCallback)로 리다이렉트됨.
        //    [HttpGet("auth/google-callback")]
        //    public async Task<IActionResult> GoogleCallback()
        //    {
        //        // 미들웨어가 만든 사용자 주체에서 클레임 꺼내기
        //        var email = User.FindFirstValue(ClaimTypes.Email);
        //        var name = User.FindFirstValue(ClaimTypes.Name);

        //        if (string.IsNullOrEmpty(email))
        //        {
        //            // 드물게 User가 비어있으면 인증 상태를 다시 확인
        //            var auth = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        //            email = auth?.Principal?.FindFirst(ClaimTypes.Email)?.Value;
        //            name = auth?.Principal?.FindFirst(ClaimTypes.Name)?.Value;
        //        }

        //        if (!string.IsNullOrEmpty(email))
        //        {
        //            var user = await _dbContext.Users.FindAsync(email);
        //            if (user == null)
        //            {
        //                _dbContext.Users.Add(new User
        //                {
        //                    Email = email,
        //                    Name = name,
        //                    Provider = "Google",
        //                    CreatedAt = DateTime.Now,
        //                    LastLogin = DateTime.Now
        //                });
        //            }
        //            else
        //            {
        //                user.LastLogin = DateTime.Now;
        //            }

        //            await _dbContext.SaveChangesAsync();
        //            return RedirectToAction("Welcome");
        //        }

        //        return RedirectToAction("Index");
        //    }

        //    public IActionResult Welcome() => View();
        #endregion
    }


}

