
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
                    opt.PermitLimit = 10;                      // 10ȸ
                    opt.Window = TimeSpan.FromSeconds(10);     // 10��
                    opt.QueueLimit = 0;
                    opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });
            });

            // Serilog ����
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day) // ��¥�� �α�����
                .WriteTo.File(                                      // error ����
        "Logs/error/error-.txt",
        restrictedToMinimumLevel: LogEventLevel.Error,  // Error �̻� ���
        rollingInterval: RollingInterval.Day
        ).CreateLogger();

            // ASP.NET Core �⺻ �α� ��� Serilog ���
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
            app.UseExceptionHandler("/error"); // �ͼ��� �ڵ� ���ϸ� �������� �����̷�Ʈ

            // �����̷�Ʈ ĳġ
            app.Map("/error", (HttpContext context, ILogger<Program> logger) =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                var ex = feature?.Error;

                // ?? ���⼭ Serilog ILogger ��� �� �ڵ����� error���� �α׷� �����
                logger.LogError(ex, "Unhandled exception occurred: {Message}", ex?.Message);

                // Ŭ���̾�Ʈ ����
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
