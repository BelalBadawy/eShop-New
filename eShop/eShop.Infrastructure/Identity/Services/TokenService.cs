using eShop.Application.Features.Token;
using eShop.Application.Features.Token.Queries;
using eShop.Application.Models;
using eShop.Application.Models.JWT;
using eShop.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace eShop.Infrastructure.Identity.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly JwtConfiguration _tokenSettings;

        public TokenService(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<JwtConfiguration> tokenSettings)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenSettings = tokenSettings.Value;
        }

        public async Task<IResponseWrapper> GetTokenAsync(TokenRequest tokenRequest)
        {
            #region Validations
            var userInDb = await _userManager.FindByEmailAsync(tokenRequest.Email);
            if (userInDb == null)
            {
                return await ResponseWrapper.FailAsync(message: "Invalid Credentials.");
            }
            // Check if Active
            if (!userInDb.IsActive)
            {
                return await ResponseWrapper.FailAsync("User not active. Please contact the administrator");
            }
            // Check email if email confirmed
            if (!userInDb.EmailConfirmed)
            {
                return await ResponseWrapper.FailAsync("Email not confirmed.");
            }
            // Check password
            var isPasswordValid = await _userManager.CheckPasswordAsync(userInDb, tokenRequest.Password);
            if (!isPasswordValid)
            {
                // Increment failed access count (optional if lockout enabled)
                await _userManager.AccessFailedAsync(userInDb);

                return await ResponseWrapper.FailAsync("Invalid Credentials.");
            }

            // Check if locked out
            if (await _userManager.IsLockedOutAsync(userInDb))
            {
                return await ResponseWrapper.FailAsync("Account is locked. Please try again later or contact support.");
            }

            #endregion

            // Reset failed access count after successful login
            await _userManager.ResetAccessFailedCountAsync(userInDb);

            // Generate token

            userInDb.RefreshToken = GenerateRefreshToken();
            userInDb.RefreshTokenExpiryDate = DateTime.Now.AddDays(_tokenSettings.RefreshTokenExpiryInDays);

            await _userManager.UpdateAsync(userInDb);

            // Generate auth token
            var token = await GenerateJwtAsync(userInDb);

            var tokenResponse = new TokenResponse
            {
                Token = token,
                RefreshToken = userInDb.RefreshToken,
                RefreshTokenExpiryTime = userInDb.RefreshTokenExpiryDate
            };

            return await ResponseWrapper<TokenResponse>.SuccessAsync(data: tokenResponse);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();
            var permissionClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
                var currentRole = await _roleManager.FindByNameAsync(role);
                var allPermissionsForCurrentRole = await _roleManager.GetClaimsAsync(currentRole);

                permissionClaims.AddRange(allPermissionsForCurrentRole);
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.FullName),
                new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
            }
            .Union(roleClaims)
            .Union(userClaims)
            .Union(permissionClaims);

            return claims;
        }

        private SigningCredentials GetSigningCredentials()
        {
            var secret = Encoding.UTF8.GetBytes(_tokenSettings.Secret);
            return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
        }

        private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {
            var token = new JwtSecurityToken(
                issuer: _tokenSettings.Issuer,
                audience: _tokenSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_tokenSettings.TokenExpiryInMunites),
                signingCredentials: signingCredentials);
            var tokenHandler = new JwtSecurityTokenHandler();
            var encryptedToken = tokenHandler.WriteToken(token);
            return encryptedToken;
        }

        private async Task<string> GenerateJwtAsync(ApplicationUser user)
        {
            var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));
            return token;
        }

        public async Task<IResponseWrapper> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            var userPrincipal = GetClaimPrincipalFromExpiredToken(refreshTokenRequest.Token);
            var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);

            var userInDb = await _userManager.FindByEmailAsync(userEmail);
            if (userInDb is not null)
            {
                if (userInDb.RefreshToken != refreshTokenRequest.RefreshToken
                    || userInDb.RefreshTokenExpiryDate <= DateTime.Now)
                {
                    return await ResponseWrapper.FailAsync(message: "Invalid token provided.");
                }

                var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(userInDb));
                userInDb.RefreshToken = GenerateRefreshToken();
                userInDb.RefreshTokenExpiryDate = DateTime.Now.AddDays(_tokenSettings.RefreshTokenExpiryInDays);

                await _userManager.UpdateAsync(userInDb);

                var tokenResponse = new TokenResponse
                {
                    Token = token,
                    RefreshToken = userInDb.RefreshToken,
                    RefreshTokenExpiryTime = userInDb.RefreshTokenExpiryDate
                };

                return await ResponseWrapper<TokenResponse>.SuccessAsync(tokenResponse);
            }
            return await ResponseWrapper.FailAsync(message: "User does not exist.");
        }

        private ClaimsPrincipal GetClaimPrincipalFromExpiredToken(string expiredToken)
        {
            var tokenValidationParms = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidIssuer = _tokenSettings.Issuer,
                ValidAudience = _tokenSettings.Audience,
                RoleClaimType = ClaimTypes.Role,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Secret)),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(expiredToken, tokenValidationParms, out var securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken
                || !jwtSecurityToken.Header.Alg
                .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
