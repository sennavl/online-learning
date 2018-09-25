using Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Project.Utils
{
    public class PasswordHelper
    {
        public static bool IsValid(string email, string password)
        {
            using (var db = new OnlineLearningDataContext())
            {
                return db.Users.Any(u => u.email == email
                    && u.password == EncodePasswordMd5(password));
            }
        }

        public static string EncodePasswordMd5(string pass) //Encrypt using MD5    
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            Byte[] originalBytes = ASCIIEncoding.Default.GetBytes(pass);
            Byte[] encodedBytes = md5.ComputeHash(originalBytes);

            var str = BitConverter.ToString(encodedBytes);
            var charsToRemove = new string[] { "-" };
            foreach (var c in charsToRemove)
            {
                str = str.Replace(c, string.Empty);
            }

            return str.ToLower();
        }
    }
}