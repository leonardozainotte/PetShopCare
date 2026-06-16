using System.Security.Cryptography;
using System.Text;

namespace PetShopCare.Services {
    public static class CriptografiaService {
        public static string GerarHash(string senhaAberto) {
            if (string.IsNullOrWhiteSpace(senhaAberto)) return string.Empty;

            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(senhaAberto));

            var builder = new StringBuilder();
            foreach (var b in bytes) {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}