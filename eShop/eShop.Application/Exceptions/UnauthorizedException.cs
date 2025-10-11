using eShop.Application.Exceptions;
using System.Net;

namespace eShopApplication.Exceptions
{
    public class UnauthorizedException : DomainException
    {
        public UnauthorizedException(string message)
            : base(message, "Unauthorized", 401) { }
    }
}
