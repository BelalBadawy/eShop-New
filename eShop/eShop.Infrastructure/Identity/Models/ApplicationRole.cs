using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace eShop.Infrastructure.Identity.Models
{
    public class ApplicationRole : IdentityRole<int>
    {
        [MaxLength(256)]
        public string Description { get; set; } = string.Empty;
    }
}
