
namespace eShop.Application.Exceptions
{
    public class ConflictException : DomainException
    {
        public ConflictException(string message)
            : base(message, "Conflict", 409) { }
    }
}
