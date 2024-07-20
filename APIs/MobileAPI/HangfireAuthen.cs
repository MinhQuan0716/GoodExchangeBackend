using Hangfire.Annotations;
using Hangfire.Dashboard;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace MobileAPI
{
    public class HangfireAuthen : IDashboardAuthorizationFilter
    {
        private readonly string _secretKey;
        public HangfireAuthen(string secretKey)
        {

            _secretKey = secretKey;

        }
        public bool Authorize([NotNull] DashboardContext context)
        {
            /*var httpContext = context.GetHttpContext();
            var authHeader = httpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = System.Text.Encoding.ASCII.GetBytes(_secretKey);
                try
                {
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    }, out SecurityToken validatedToken);

                    var jwtToken = (JwtSecurityToken)validatedToken;
                    var userId = jwtToken.Claims.First(x => x.Type == "id").Value;

                    if(userId == null)
                    {
                        return false;
                    }
                    // Optionally check for roles or other claims
                    return true;
                }
                catch
                {
                    return false;
                }
            }*/

            return true;
        }
    }
}
