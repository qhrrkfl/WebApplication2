
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.RateLimiting;
using Serilog;
using Serilog.Events;
using System.Threading.RateLimiting;

namespace DGRBWepAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            // Add services to the container.



            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = 429; // Too Many Requests
                options.AddFixedWindowLimiter("PerIpPolicy", opt =>
                {
                    opt.PermitLimit = 10;                      // 10회
                    opt.Window = TimeSpan.FromSeconds(10);     // 10초
                    opt.QueueLimit = 0;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
            });

            // Serilog 설정
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // 날짜별 로그파일
                .WriteTo.File(                                      // error 파일
        "Logs/error/error-.txt",
        restrictedToMinimumLevel: LogEventLevel.Error,  // Error 이상만 기록
        rollingInterval: RollingInterval.Day
        ).CreateLogger();

            // ASP.NET Core 기본 로깅 대신 Serilog 사용
            builder.Host.UseSerilog();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();
            app.UseExceptionHandler("/error"); // 익셉션 핸들 못하면 이쪽으로 리다이렉트

            // 리다이렉트 캐치
            app.Map("/error", (HttpContext context, ILogger<Program> logger) =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                var ex = feature?.Error;

                // ?? 여기서 Serilog ILogger 사용 → 자동으로 error폴더 로그로 저장됨
                logger.LogError(ex, "Unhandled exception occurred: {Message}", ex?.Message);

                // 클라이언트 응답
                return Results.Problem(
                    title: "Internal Server Error",
                    detail: ex?.Message,
                    statusCode: 500);
            });


            app.MapControllers();

            app.Run();
        }
    }
}
