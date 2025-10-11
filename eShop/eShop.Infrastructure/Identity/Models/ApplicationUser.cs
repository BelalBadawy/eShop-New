using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace eShop.Infrastructure.Identity.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        [MaxLength(256)]
        public string FullName { get; set; } = string.Empty;
        [MaxLength(256)]
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime RefreshTokenExpiryDate { get; set; }
        public bool IsActive { get; set; }
    }
}
