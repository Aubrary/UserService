using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserService.Data;
using UserService.Entities;
using UserService.Helpers;
using UserService.Interfaces;

namespace UserService.Services
{

    public class UserLoginService : IUserLoginService
    {
        // users hardcoded for testing
        private List<User> _users = new List<User>
        { 
            new User { Id = new Guid("e7a23854-d560-4dd2-9d06-e7dbac39b50a"), FirstName = "Admin", LastName = "User", Username = "admin", Password = "admin", Role = Role.Admin },
            new User { Id = new Guid("70145a01-54fe-4559-99a5-8ab0cea951a4"), FirstName = "Normal", LastName = "User", Username = "user", Password = "user", Role = Role.User } 
        };

        private readonly AppSettings _appSettings;
        protected readonly UserServiceContext Context;

        public UserLoginService(UserServiceContext context, IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
            Context = context;
        }

        public User Authenticate(string username, string password)
        {
            var user = Context.Users.FirstOrDefault(e => e.Username == username && e.Password == password);

            // return null if user not found
            if (user == null)
                return null;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            user.Password = null;

            return user;
        }

        public User MapToEntity(User user, User entity = null)
        {
            if (entity == null)
            {
                entity = new User();
            }

            entity.Username = user.Username;
            entity.Password = user.Password;
            entity.FirstName = user.FirstName;
            entity.LastName = user.LastName;
            entity.Role = user.Role;

            return entity;
        }

        public IEnumerable<User> GetAll()
        {
            
            return Context.Users;
        }

        public User GetById(Guid id) {
            var user = Context.Users.FirstOrDefault(e => e.Id == id);

            // return user without password
            if (user != null) 
                user.Password = null;

            return user;
        }

        public static User WithoutPassword(User user)
        {
            user.Password = null;
            return user;
        }

    }
}