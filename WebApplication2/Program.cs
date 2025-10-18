using DBconnect.Models;
using DBconnection;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Service;

using Microsoft.Extensions.Configuration;

namespace WebApplication2
{
    public class Program
    {
        public static void Main(string[] args)
        {

           

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();


            builder.Services.AddDbContext<TranslateDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("TranslateDB")));

            
            builder.Services.AddScoped<AccountService>();       // Scoped
            builder.Services.AddScoped<MailValService>();       // Scoped
            builder.Services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();


            //// 인증 (쿠키 + 구글)
            //builder.Services.AddAuthentication(options =>
            //{
            //    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;   // 로그인 상태 유지
            //    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;        // 로그인 버튼 시 구글로 챌린지
            //})
            //.AddCookie()
            //.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            //{
            //    var google = builder.Configuration.GetSection("Authentication:Google");
            //    options.ClientId = google["ClientId"];
            //    options.ClientSecret = google["ClientSecret"];
            //    // 콜백 경로는 기본값 /signin-google (Google 콘솔에 등록한 그것)
            //});

            /// 로그인 직접 구현
            builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme; // ✅ 중요
            })
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";          // ✅ 인증 안 됐을 때 보낼 URL
                options.AccessDeniedPath = "/Account/Denied";  // ✅ 권한 부족일 때
                options.SlidingExpiration = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(1);
            });

            // ✅ 기본(폴백) 권한 정책 = 인증된 사용자만
            builder.Services.AddAuthorization(options =>
            {
                options.FallbackPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
            });


            var app = builder.Build();

            var debug = ((IConfigurationRoot)app.Services.GetRequiredService<IConfiguration>()).GetDebugView();
            app.Logger.LogInformation("CONFIG DEBUG:\n{cfg}", debug);


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();




            //======================================================== // 이건 순서 중요 미들웨어
            app.UseRouting();
            app.UseAuthentication();   // ✅ 순서 중요: 인증 먼저

            // validation 안한놈을 redirect시켜버리는 미들웨어
            app.Use(async (ctx, next) =>
            {
                var endpoint = ctx.GetEndpoint();
                var allowsAnon = endpoint?.Metadata
                    .GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>() != null;

                var isAuthed = ctx.User?.Identity?.IsAuthenticated == true;
                var isValidated = ctx.User?.FindFirst("validated")?.Value == "true";

                // 이미 Validation 페이지면 루프 방지
                var onValidation = ctx.Request.Path.StartsWithSegments("/Account/Validation");

                if (isAuthed && !isValidated && !allowsAnon && !onValidation)
                {
                    ctx.Response.Redirect("/Account/Validation");
                    return;
                }

                await next();
            });


            app.UseAuthorization();
            //=======================================================

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

          

            app.Run();
        }
    }
}
