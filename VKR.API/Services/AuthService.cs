using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VKR.API.Configs;
using VKR.API.Exceptions;
using VKR.API.Models.Token;
using VKR.Common;
using VKR.Common.Const;
using VKR.DAL;
using VKR.DAL.Entities;

namespace VKR.API.Services
{
    public class AuthService
    {
        private readonly DataContext _context;
        private readonly AuthConfig _authConfig;

        public AuthService(
            DataContext context,
            IOptions<AuthConfig> config)
        {
            _context = context;
            _authConfig = config.Value;
        }

        public async Task<TokenModel> GetTokens(string login, string password)
        {
            var user = await GetUserByCredention(login, password);
            var session = await _context.Sessions.AddAsync(new VKR.DAL.Entities.UserSession
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Created = DateTime.UtcNow,
                RefreshToken = Guid.NewGuid()
            });
            await _context.SaveChangesAsync();
            return GenerateTokens(session.Entity);
        }

        public async Task<TokenModel> GetTokensByRefreshToken(string refreshToken)
        {
            var validParams = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                IssuerSigningKey = _authConfig.SimmetricSecurityKey()
            };

            var principal = new JwtSecurityTokenHandler().ValidateToken(refreshToken, validParams, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtToken
                || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            if (principal.Claims.FirstOrDefault(c => c.Type == ClaimNames.RefreshToken)?.Value is String refreshIdString
                && Guid.TryParse(refreshIdString, out var refreshId))
            {
                var session = await GetSessionByRefreshToken(refreshId);
                if (!session.IsActive)
                {
                    throw new Exception("session is not active");
                }

                var user = session.User;

                session.RefreshToken = Guid.NewGuid();
                await _context.SaveChangesAsync();

                return GenerateTokens(session);
            }
            else
            {
                throw new SecurityTokenException("Invalid token");
            }
        }

        public async Task<UserSession> GetSessionById(Guid sessionId)
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
            if (session == null)
            {
                throw new Exception("session is not found");
            }
            return session;
        }

        public async Task RemoveSession(Guid sessionId)
        {
            var session = await GetSessionById(sessionId);
            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
        }




        private async Task<UserSession> GetSessionByRefreshToken(Guid reshreshTokenId)
        {
            var session = await _context.Sessions.Include(x => x.User).FirstOrDefaultAsync(s => s.RefreshToken == reshreshTokenId);
            if (session == null)
            {
                throw new Exception("session is not found");
            }
            return session;
        }

        private TokenModel GenerateTokens(VKR.DAL.Entities.UserSession session)
        {
            var dtNow = DateTime.Now;

            var jwt = new JwtSecurityToken(
                issuer: _authConfig.Issuer,
                audience: _authConfig.Audience,
                notBefore: dtNow,
                claims: new Claim[]
                {
                    new Claim(ClaimsIdentity.DefaultNameClaimType,session.User.Name),
                    new Claim(ClaimNames.SessionId,session.Id.ToString()),
                    new Claim(ClaimNames.UserId,session.User.Id.ToString()),
                },
                expires: DateTime.Now.AddMinutes(_authConfig.LifeTime),
                signingCredentials: new SigningCredentials(_authConfig.SimmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var refresh = new JwtSecurityToken(
                notBefore: dtNow,
                claims: new Claim[]
                {
                    new Claim(ClaimNames.RefreshToken,session.RefreshToken.ToString()),
                },
                expires: DateTime.Now.AddHours(_authConfig.LifeTime),
                signingCredentials: new SigningCredentials(_authConfig.SimmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedRefresh = new JwtSecurityTokenHandler().WriteToken(refresh);

            return new TokenModel(encodedJwt, encodedRefresh);

        }

        private async Task<VKR.DAL.Entities.User> GetUserByCredention(string login, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == login.ToLower());
            if (user == null)
            {
                throw new NotFoundException(login);
            }
            if (!HashHelper.Verify(password, user.PasswordHash))
            {
                throw new ArgumentException("Password Is Incorrect");
            }
            return user;

        }






    }
}
