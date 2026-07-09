namespace Peliculas.Services
{
    public interface IEncoderService
    {
        string Encrypt(string toEncrypt);
        bool Verify(string toVerify, string hash);
    }
    public class EncoderService : IEncoderService
    {
        public string Encrypt(string toEncrypt)
        {
            string salt = BCrypt.Net.BCrypt.GenerateSalt(13);
            string hashed = BCrypt.Net.BCrypt.HashPassword(toEncrypt, salt);
            return hashed;
        }
        public bool Verify(string toVerify, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(hash, toVerify);
        }
    }
}
