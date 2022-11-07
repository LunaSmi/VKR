using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using VKR.API.Configs;
using VKR.API.Models;
using VKR.Common;
using VKR.DAL;
using VKR.DAL.Entities;

namespace VKR.API.Services
{
    public class UsersService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly AuthConfig _authConfig;

        public UsersService(
            DataContext context,
            IMapper mapper,
            IOptions<AuthConfig> config)
        {
            _context = context;
            _mapper = mapper;
            _authConfig = config.Value;
        }

        public async Task AddAvatarToUser(Guid userId,MetaDataModel meta,string filePath)
        {
            var user = await _context.Users.Include(u => u.Avatar).FirstOrDefaultAsync(x=>x.Id==userId);
            if(user != null)
            {
                var avatar = new Avatar
                {
                    Owner = user,
                    MimeType = meta.MimeType,
                    Name = meta.Name,
                    Size = meta.Size,
                    FilePath = filePath,
                };
                user.Avatar = avatar;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<AttachModel> GetUserAvatar(Guid userId)
        {
            var user = await GetUserById(userId);
            var attach = _mapper.Map<AttachModel>(user.Avatar);
            return attach;
        }

        public async Task<bool> CheckUserExistAsync(string email)
        {
            return await _context.Users.AnyAsync(x=>x.Email.ToLower() == email.ToLower());
        }

        public async Task Delete(Guid id)
        {
            var dbUser = await GetUserById(id);
            if(dbUser != null)
            {
                _context.Users.Remove(dbUser);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Guid> CreateUser(CreateUserModel createUserModel)
        {
            var dbuser = _mapper.Map<VKR.DAL.Entities.User>(createUserModel);
            var user = await _context.Users.AddAsync(dbuser);
            await _context.SaveChangesAsync();
            return user.Entity.Id;
        }

        public async Task<List<UserModel>> GetUsers()
        {
            return await _context.Users.AsNoTracking().ProjectTo<UserModel>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<UserModel> GetUser(Guid id)
        {
            var user = await GetUserById(id);
            return _mapper.Map<UserModel>(user);
        }




        public async Task<TokenModel> GetTokens(string login, string password)
        {
            var user = await GeyUserByCredention(login, password);
            var session = await _context.Sessions.AddAsync(new VKR.DAL.Entities.UserSession
            {
                Id= Guid.NewGuid(),
                UserId = user.Id,
                Created = DateTime.UtcNow,
                RefreshToken = Guid.NewGuid()
            });
            await _context.SaveChangesAsync();
            return GenerateTokens(session.Entity);
        }

        public async Task<UserSession> GetSessionById(Guid id)
        {
            var session = await _context.Sessions.FirstOrDefaultAsync(s=> s.Id == id);
            if (session == null)
            {
                throw new Exception("session is not found");
            }
            return session;
        }

        private async Task<UserSession> GetSessionByRefreshToken(Guid id)
        {
            var session = await _context.Sessions.Include(x=>x.User).FirstOrDefaultAsync(s => s.RefreshToken == id);
            if (session == null)
            {
                throw new Exception("session is not found");
            }
            return session;
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

            if(securityToken is not JwtSecurityToken jwtToken 
                || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            if(principal.Claims.FirstOrDefault(c=>c.Type == "refreshToken")?.Value is String refreshIdString
                && Guid.TryParse(refreshIdString,out var refreshId) )
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
                    new Claim("sessionId",session.Id.ToString()),
                    new Claim("userId",session.User.Id.ToString()),
                },
                expires: DateTime.Now.AddMinutes(_authConfig.LifeTime),
                signingCredentials: new SigningCredentials(_authConfig.SimmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var refresh = new JwtSecurityToken(
                notBefore: dtNow,
                claims: new Claim[]
                {
                    new Claim("refreshToken",session.RefreshToken.ToString()),
                },
                expires: DateTime.Now.AddHours(_authConfig.LifeTime),
                signingCredentials: new SigningCredentials(_authConfig.SimmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedRefresh = new JwtSecurityTokenHandler().WriteToken(refresh);

            return new TokenModel(encodedJwt, encodedRefresh);

        }

        private async Task<VKR.DAL.Entities.User> GetUserById(Guid id)
        {
            var user = await _context.Users.Include(x => x.Avatar).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user),"User Not Found");
            }
            return user;
        }

        private async Task<VKR.DAL.Entities.User> GeyUserByCredention(string login, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == login.ToLower());
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user),"User Not Found");
            }
            if (!HashHelper.Verify(password, user.PasswordHash))
            {
                throw new ArgumentException("Password Is Incorrect");
            }
            return user;

        }
    }
}
