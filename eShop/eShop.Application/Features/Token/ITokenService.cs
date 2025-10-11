namespace eShop.Application.Features.Token
{
    public interface ITokenService
    {
        Task<IResponseWrapper> GetTokenAsync(TokenRequest tokenRequest);
        Task<IResponseWrapper> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest);
    }
}
