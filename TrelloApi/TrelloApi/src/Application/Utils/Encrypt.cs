using System.Security.Cryptography;
using System.Text;

namespace TrelloApi.Application.Utils;

public class Encrypt: IEncrypt
{
    public string HashPassword(string password)
    {
        SHA256 sha256 = SHA256.Create();
        ASCIIEncoding encoding = new ASCIIEncoding();
        byte[] stream = sha256.ComputeHash(encoding.GetBytes(password));
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < stream.Length; i++)
        {
            sb.AppendFormat("{0:x2}", stream[i]);
        }

        return sb.ToString();
    }

    public bool ComparePassword(string passwordWithoutEncryption, string passwordWithEncryption)
    {
        string hashedPassword = HashPassword(passwordWithoutEncryption);
        return hashedPassword.Equals(passwordWithEncryption);
    }
}