
using eShop.Application.Models;

namespace eShop.Application.Interfaces
{
    public interface IEmailService
    {
        Task<string> SendAsync(SendEmailDto request);
    }
}
