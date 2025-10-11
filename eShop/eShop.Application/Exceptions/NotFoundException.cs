
namespace eShop.Application.Exceptions
{
    public class NotFoundException : DomainException
    {
        public NotFoundException(string resourceName, object key) : base($"{resourceName} with id {key} was not found", "Not Found", 404) { }
    }
}
