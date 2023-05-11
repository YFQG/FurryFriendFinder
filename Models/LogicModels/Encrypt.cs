using System.Security.Cryptography;
using System.Text;

namespace FurryFriendFinder.Models.LogicModels
{
    public class Encrypt
    {
        //This method can encrypt the user´s password 
        public static string GetSHA256(string str)
        {
            if (str == null) { return ""; }
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(str));
            for (int i = default; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
        }
    }
}
