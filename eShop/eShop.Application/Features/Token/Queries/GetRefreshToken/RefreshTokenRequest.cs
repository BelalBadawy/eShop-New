namespace eShop.Application.Features.Token.Queries
{
    public class RefreshTokenRequest
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
