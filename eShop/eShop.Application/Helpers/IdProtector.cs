using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace eShop.Application.Helpers
{
    public class IdProtector
    {
        private readonly IDataProtector _protector;

        public IdProtector(IDataProtectionProvider provider, IConfiguration configuration)
        {
            // Unique purpose string → isolates keys per usage
            _protector = provider.CreateProtector(configuration["IdProtection:SecretKey"]);
        }

        public string Protect(int id) => _protector.Protect(id.ToString());

        //public int Unprotect(string protectedId)
        //{

        //    int.Parse(_protector.Unprotect(protectedId));
        //}

        public bool TryUnprotect(string protectedId, out int id)
        {
            id = default;

            if (string.IsNullOrWhiteSpace(protectedId))
                return false;

            try
            {
                var unprotected = _protector.Unprotect(protectedId);
                return int.TryParse(unprotected, out id);
            }
            catch
            {
                // log if needed
                return false;
            }
        }
    }
}
