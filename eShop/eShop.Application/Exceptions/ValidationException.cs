using FluentValidation.Results;

namespace eShop.Application.Exceptions
{
    public class ValidationException : DomainException
    {
        public Dictionary<string, string[]> Errors { get; }

        public ValidationException(Dictionary<string, string[]> errors)
            : base("One or more validation errors occurred", "Validation Error", 400)
        {
            Errors = errors;
        }
    }
}
