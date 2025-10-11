using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace eShop.Infrastructure.Identity.Models
{
    public class ApplicationRoleClaim : IdentityRoleClaim<int>
    {
        [MaxLength(256)]
        public string Description { get; set; } = string.Empty;

    }
}
