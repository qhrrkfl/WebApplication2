using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;


namespace KeyGen
{
    internal class Program
    {
        

        static void Main(string[] args)
        {
            // 암호화 키 젠
            var keyb64 = CreateKey();
            byte[] bkey = Convert.FromBase64String(keyb64);
            Console.WriteLine(keyb64);
            Console.WriteLine("===");
            
            string proctectpass = Protect("", bkey);
            Console.WriteLine(proctectpass);
            
            
            Console.WriteLine("-=== 복호화 검증");
            Console.WriteLine(Unprotect(proctectpass, bkey));



        }
        /// <summary>
        /// 32 bytes from ENV/Secret Manager
        /// </summary>
        /// <param name="plain">알지?</param>
        /// <param name="key">32 bytes from ENV/Secret Manager</param>
        /// <returns></returns>
        static public string Protect(string plain , byte[] key)
        {
            var nonce = RandomNumberGenerator.GetBytes(12);
            var pt = Encoding.UTF8.GetBytes(plain);
            var ct = new byte[pt.Length];
            var tag = new byte[16];
            using var gcm = new AesGcm(key);
            gcm.Encrypt(nonce, pt, ct, tag);
            return Convert.ToBase64String(
                nonce.Concat(tag).Concat(ct).ToArray()); // 저장
        }

        static public string Unprotect(string b64,byte[] key)
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



        static string CreateKey()
        {
            // 32바이트(256비트) 키 생성
            byte[] key = RandomNumberGenerator.GetBytes(32);

            // 보관/설정 파일용 Base64 문자열 (길이 44, 끝에 '=' 패딩 1개)
            string keyB64 = Convert.ToBase64String(key);

            return keyB64;
        }
    }
}
