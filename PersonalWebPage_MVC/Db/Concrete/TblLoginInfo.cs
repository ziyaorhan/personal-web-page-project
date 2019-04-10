using PersonalWebPage_MVC.Db.Abstract;
using PersonalWebPage_MVC.Models;
using System.Security.Cryptography;
using System.Text;

namespace PersonalWebPage_MVC.Db.Concrete
{
    public class TblLoginInfo : DatabaseOperations<LoginInfo, PersonPageContext>
    {
        public static int x = 0;
        public string EncryptSha(string text, int repeatCount = 11)
        {
            try
            {
                x++;
                SHA512 sha = new SHA512CryptoServiceProvider();
                byte[] shaAsByte = sha.ComputeHash(Encoding.UTF8.GetBytes(text));
                StringBuilder sb = new StringBuilder();
                foreach (var item in shaAsByte)
                {
                    sb.Append(item.ToString("x2"));
                }
                if (x <= repeatCount)
                {
                    EncryptSha(sb.ToString(), repeatCount);
                }
                return sb.ToString();
            }
            catch 
            {
                return "";
            }
        }
        public string EncryptMd5(string text, int repeatCount = 11)
        {
            try
            {
                x++;
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] shaAsByte = md5.ComputeHash(Encoding.UTF8.GetBytes(text));
                StringBuilder sb = new StringBuilder();
                foreach (var item in shaAsByte)
                {
                    sb.Append(item.ToString("x2"));
                }
                if (x <= repeatCount)
                {
                    EncryptMd5(sb.ToString(), repeatCount);
                }
                return sb.ToString();
            }
            catch 
            {
                return "";
            }
        }
    }
}