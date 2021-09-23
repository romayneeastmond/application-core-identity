using ApplicationCoreIdentity.Models;
using ApplicationCoreIdentity.Models.Model.Administration;
using ApplicationCoreIdentity.Services.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCoreIdentity.Services.Authentication
{
    public class AuthenticationService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IAuthenticationConfiguration _authenticationConfiguration;
        private readonly ApplicationDbContext _db;

        public AuthenticationService(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IAuthenticationConfiguration authenticationConfiguration, ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authenticationConfiguration = authenticationConfiguration;
            _db = db;
        }

        public async Task<UserViewModel> Login(string emailAddress, string password, bool rememberMe)
        {
            if (string.IsNullOrWhiteSpace(emailAddress) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentNullException();
            }

            var result = await _signInManager.PasswordSignInAsync(emailAddress, password, rememberMe, false);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(emailAddress);
                var databaseUser = await _db.Users.Where(x => x.EmailAddress == emailAddress).FirstOrDefaultAsync();

                if (user == null || databaseUser == null)
                {
                    throw new Exception();
                }

                return new UserViewModel
                {
                    Id = databaseUser.Id,
                    FirstName = databaseUser.FirstName,
                    LastName = databaseUser.LastName,
                    EmailAddress = databaseUser.EmailAddress,
                    Token = GetToken(databaseUser.EmailAddress, databaseUser.Id.ToString()),
                    Created = databaseUser.Created,
                    Updated = databaseUser.Updated
                };
            }

            throw new Exception();
        }

        public async Task Logout()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<UserViewModel> Register(UserViewModel user)
        {
            if (await _userManager.FindByEmailAsync(user.EmailAddress) != null)
            {
                throw new Exception();
            }

            var identityUser = new IdentityUser
            {
                Email = user.EmailAddress,
                UserName = user.EmailAddress
            };

            var result = await _userManager.CreateAsync(identityUser, user.Password);

            if (result.Succeeded)
            {
                var databaseUser = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailAddress = user.EmailAddress,
                    Active = true,
                    Created = DateTime.UtcNow,
                    Updated = DateTime.UtcNow
                };

                _db.Users.Add(databaseUser);

                await _db.SaveChangesAsync();

                return new UserViewModel
                {
                    Id = databaseUser.Id,
                    FirstName = databaseUser.FirstName,
                    LastName = databaseUser.LastName,
                    EmailAddress = databaseUser.EmailAddress,
                    Token = GetToken(databaseUser.EmailAddress, databaseUser.Id.ToString()),
                    Created = databaseUser.Created,
                    Updated = databaseUser.Updated
                };
            }

            throw new Exception(string.Join(Environment.NewLine, result.Errors.Select(x => x.Description)));
        }

        public async Task Update(UserViewModel user)
        {
            var result = await _signInManager.PasswordSignInAsync(user.EmailAddress, user.Password, false, false);

            if (!result.Succeeded)
            {
                throw new Exception();
            }

            var identityUser = await _userManager.FindByEmailAsync(user.EmailAddress);

            if (identityUser != null)
            {
                var databaseUser = await _db.Users.Where(x => x.EmailAddress == user.EmailAddress).FirstOrDefaultAsync();

                if (databaseUser != null)
                {
                    databaseUser.FirstName = user.FirstName;
                    databaseUser.LastName = user.LastName;
                    databaseUser.Updated = DateTime.UtcNow;

                    await _db.SaveChangesAsync();
                }
            }
        }

        public async Task UpdatePassword(UserViewModel user, string oldPassword)
        {
            var result = await _signInManager.PasswordSignInAsync(user.EmailAddress, oldPassword, false, false);

            if (!result.Succeeded)
            {
                throw new Exception();
            }

            var identityUser = await _userManager.FindByEmailAsync(user.EmailAddress);

            if (identityUser == null)
            {
                throw new Exception();
            }

            var passwordValidator = new PasswordValidator<IdentityUser>();

            var resultPasswordValidator = await passwordValidator.ValidateAsync(_userManager, identityUser, user.Password);

            if (!resultPasswordValidator.Succeeded)
            {
                throw new Exception(string.Join(Environment.NewLine, resultPasswordValidator.Errors.Select(x => x.Description)));
            }

            identityUser.PasswordHash = _userManager.PasswordHasher.HashPassword(identityUser, user.Password);

            var resultUpdate = await _userManager.UpdateAsync(identityUser);

            if (!resultUpdate.Succeeded)
            {
                throw new Exception(string.Join(Environment.NewLine, resultUpdate.Errors.Select(x => x.Description)));
            }
        }

        private string GetToken(string emailAddress, string id)
        {
            var claims = new[]
                {
                    new Claim(JwtRegisteredClaimNames.Email, emailAddress),
                    new Claim(JwtRegisteredClaimNames.Jti, id)
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationConfiguration.Key));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _authenticationConfiguration.Issuer,
                _authenticationConfiguration.Audience,
                claims,
                DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
