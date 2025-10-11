namespace eShop.Application.Exceptions
{
    public abstract class DomainException : Exception
    {
        public string Title { get; }
        public int StatusCode { get; }

        protected DomainException(string message, string title, int statusCode)
            : base(message)
        {
            Title = title;
            StatusCode = statusCode;
        }
    }
}
