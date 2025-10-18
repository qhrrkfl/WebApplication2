using DBconnect.Models;
using DBConnect;
using DBconnection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Service
{
    public enum loginType
    {
        Success,
        Failed,
        Validation
    }
    public class AccountService
    {
        private readonly TranslateDbContext _db;
        private readonly IPasswordHasher<User> _hasher;

        

        public AccountService(TranslateDbContext db, IPasswordHasher<User> hasher)
        {
            _db = db;
            _hasher = hasher;
        }

        public async Task<User> ValidateLogin(string email, string rawPass ,  CancellationToken ct )
        {
            var user = await _db.Users.FirstOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()),ct);
            if (user != null)
            {
                var ret = _hasher.VerifyHashedPassword(user, user.HashPassword, rawPass);
                if (ret == PasswordVerificationResult.Success || ret == PasswordVerificationResult.SuccessRehashNeeded)
                {
                    return user;
                }
            }
            return null;
        }

        public async Task<bool> RegisterID(string email, string rawPass, string nickName , CancellationToken ct)
        {
            bool result = false;
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(rawPass))
                return false;



            User New = new User();
            New.NickName = nickName;
            New.Email = email;
            New.HashPassword = _hasher.HashPassword(New, rawPass);

            await _db.Users.AddAsync(New , ct);

            try
            {
                var ret = await _db.SaveChangesAsync(ct);
                if (ret > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (DbUpdateException)
            {
                // 유니크 충돌 가능성 높음
                return false;
            }
        }





    }
}
