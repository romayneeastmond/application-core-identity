using ApplicationCoreIdentity.Models;
using ApplicationCoreIdentity.Services.Authentication;
using ApplicationCoreIdentity.Services.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ApplicationCoreIdentity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IAuthenticationConfiguration _authenticationConfiguration;
        private readonly ApplicationDbContext _db;

        public AuthenticationController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IAuthenticationConfiguration authenticationConfiguration, ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authenticationConfiguration = authenticationConfiguration;
            _db = db;
        }

        [HttpPost]
        [Route(template: "login")]
        [AllowAnonymous]
        public async Task<UserViewModel> Login(string emailAddress, string password, bool rememberMe) 
            => await new AuthenticationService(_userManager, _signInManager, _authenticationConfiguration, _db).Login(emailAddress, password, rememberMe);

        [HttpPost]
        [Route(template: "logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public void Logout()
            => new AuthenticationService(_userManager, _signInManager, _authenticationConfiguration, _db).Logout();

        [HttpPost]
        [Route(template: "register")]
        [AllowAnonymous]
        public async Task<UserViewModel> Register([FromBody] UserViewModel user)
            => await new AuthenticationService(_userManager, _signInManager, _authenticationConfiguration, _db).Register(user);

        [HttpPost]
        [Route(template: "update")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public void Update([FromBody] UserViewModel user)
            => new AuthenticationService(_userManager, _signInManager, _authenticationConfiguration, _db).Update(user);

        [HttpPost]
        [Route(template: "update/password")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public void UpdatePassword([FromBody] UserViewModel user, string oldPassword)
            => new AuthenticationService(_userManager, _signInManager, _authenticationConfiguration, _db).UpdatePassword(user, oldPassword);
    }
}
