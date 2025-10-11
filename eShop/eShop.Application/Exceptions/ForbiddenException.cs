
namespace eShop.Application.Exceptions
{
    public class ForbiddenException : DomainException
    {
        public ForbiddenException(string message)
            : base(message, "Forbidden", 403) { }
    }
}
