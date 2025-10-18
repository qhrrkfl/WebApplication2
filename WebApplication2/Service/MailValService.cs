using DBConnect.TranslateDBTABLE;
using DBconnection;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using WebApplication2.Controllers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebApplication2.Service
{
    public class MailValService
    {
        private readonly TranslateDbContext _db;
        private readonly string key;
        private readonly string password;
        private readonly string passkey;
        private readonly string id;
        private readonly ILogger<MailValService> _logger;
        public MailValService(TranslateDbContext db, IConfiguration con, ILogger<MailValService> l)
        {
            _db = db;
            key = con["HashKey"]!;
            id = con["MyEmail"]!;
            passkey = con["mykey"]!;
            password = con["crypt"]!;
            _logger = l;

        }

        public async Task<string> sendEmail(string email, CancellationToken ct)
        {
            var top5 = await _db.EmailValidations.AsNoTracking().Where(x => x.Email.ToLower().Equals(email.ToLower())).OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync(ct);
            if (top5 != null)
            {
                var lasttime = top5.CreatedAt;
                var span = DateTime.Now - lasttime;
                if (span.TotalSeconds <= 60)
                {
                    return "잠시후에 시도해 주세요";
                }
                else if (span.TotalSeconds > 60)
                {
                    await SendMailHelper(email, ct);
                    return "발송";
                }
            }
            else
            {
                await SendMailHelper(email, ct);
                return "빌송";
            }
            return "unkownError";
        }


        private async Task SendMailHelper(string email, CancellationToken ct)
        {

            // 만료된 코드 발송
            Random generator = new Random();
            string strvalcode = generator.Next(0, 1000000).ToString("D6");
            byte[] data;
            using (var h = new System.Security.Cryptography.HMACSHA256(Convert.FromBase64String(key)))
            {
                data = h.ComputeHash(System.Text.Encoding.UTF8.GetBytes(strvalcode));
            }

            EmailValidations val = new EmailValidations();
            val.IsValid = false;
            val.Email = email;
            val.CreatedAt = DateTime.Now;
            val.ValCode = data;
            await using var tx = await _db.Database.BeginTransactionAsync(); // 기본: ReadCommitted

            await _db.AddAsync(val, ct);
            await _db.SaveChangesAsync(ct);

            try
            {
                await SendEMailAsync(strvalcode, email);
                await tx.CommitAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
            }
        }
        #region helper method



        static private string Unprotect(string b64, byte[] key)
        {
            var blob = Convert.FromBase64String(b64);
            var nonce = blob[..12];
            var tag = blob[12..28];
            var ct = blob[28..];
            var pt = new byte[ct.Length];
            using var gcm = new AesGcm(key);
            gcm.Decrypt(nonce, ct, tag, pt);
            return Encoding.UTF8.GetString(pt);
        }

        private async Task SendEMailAsync(string code, string targetEmail)
        {
            using SmtpClient smtp = new SmtpClient();
            using MailMessage message = new MailMessage(); 

            string plainPass = Unprotect(password, Convert.FromBase64String(passkey));

            // Naver 계정 SMTP 설정 > 
            smtp.Host = "smtp.naver.com";  //"smtp.gmail.com";
            smtp.UseDefaultCredentials = false;
            smtp.Port = 587;
            smtp.EnableSsl = true;
            
            smtp.Timeout = 20000;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.Credentials = new NetworkCredential(id , plainPass);

            //message = new MailMessage();



            message.From = new MailAddress(id, "validation center", System.Text.Encoding.UTF8);

            message.To.Add(new MailAddress(targetEmail));



            /* 기타 설정들
                message.To.Add("aaa@naver.com");   // 받는 사람
                message.CC.Add("aaa@naver.com");   // 참조
                message.Bcc.Add("aaa@naver.com");   // 비공개
                message.IsBodyHtml = false;  // Body Format
                message.Attachments.Add(new Attachment(new FileStream(@"D:\test.zip", FileMode.Open, FileAccess.Read), "test.zip"));  // File 첨부
                */

            message.Subject = "번역앱 인증코드 전송용";
            message.SubjectEncoding = System.Text.Encoding.UTF8;

            string body = string.Format("인증코드 \n {0} ", code);

            message.Body = body;
            message.BodyEncoding = System.Text.Encoding.UTF8;

            await smtp.SendMailAsync(message);


        }

        #endregion


    }
}
