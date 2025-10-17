using System.Net;
using System.Text;

namespace SmartStore.API.Middlewares
{
    public class SwaggerBasicAuthMiddleware
    {

        private readonly RequestDelegate next;

        public SwaggerBasicAuthMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;

            // Only protect /swagger and skip if it's a local request
            if (path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase)
                && !IsLocalRequest(context))
            {
                var authHeader = context.Request.Headers["Authorization"].ToString();

                // If Swagger is using Bearer token skip basic auth entirely
                if (!string.IsNullOrEmpty(authHeader) &&
                    authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                {
                    await next(context);
                    return;
                }

                // Check for Basic Auth
                if (!string.IsNullOrEmpty(authHeader) &&
                    authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                {
                    var encoded = authHeader["Basic ".Length..].Trim();
                    var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                    var parts = decoded.Split(':', 2);
                    if (parts.Length == 2 && IsAuthorized(parts[0], parts[1]))
                    {
                        await next(context);
                        return;
                    }
                }

                //  Unauthorized  stop the pipeline safely
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Swagger UI\"";
                }
                return;
            }

            // Not swagger continue normally
            await next(context);
        }

        public bool IsAuthorized(string username, string password)
        {
            // Check that username and password are correct
            return username.Equals("SmartStore2025User", StringComparison.InvariantCultureIgnoreCase)
                   && password.Equals("AdminP@ss123$%");
        }

        public bool IsLocalRequest(HttpContext context)
        {
            //Handle running using the Microsoft.AspNetCore.TestHost and the site being run entirely locally in memory without an actual TCP/IP connection
            if (context.Connection.RemoteIpAddress == null && context.Connection.LocalIpAddress == null)
            {
                return true;
            }

            if (context.Connection.RemoteIpAddress.Equals(context.Connection.LocalIpAddress))
            {
                return true;
            }

            if (IPAddress.IsLoopback(context.Connection.RemoteIpAddress))
            {
                return true;
            }

            return false;
        }
    }

    public static class SwaggerAuthorizeExtensions
    {
        public static IApplicationBuilder UseSwaggerAuthorized(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SwaggerBasicAuthMiddleware>();
        }
    }


    #region Custom
    #endregion Custom

}
