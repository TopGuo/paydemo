using System;
using System.Text;

namespace paydemo.con
{
    public class Security
    {
         public static string GetMD5(string value)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                string sign = BitConverter.ToString(md5.ComputeHash(Encoding.UTF8.GetBytes(value)));
                return sign.Replace("-", "");
            }
        }
    }
}