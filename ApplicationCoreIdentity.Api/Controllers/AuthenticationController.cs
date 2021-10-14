using ApplicationCoreIdentity.Services.Authentication;
using ApplicationCoreIdentity.Services.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ApplicationCoreIdentity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        [Route(template: "login")]
        [AllowAnonymous]
        public async Task<UserViewModel> Login(string emailAddress, string password, bool rememberMe)
            => await _authenticationService.Login(emailAddress, password, rememberMe);

        [HttpPost]
        [Route(template: "logout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public void Logout()
            => _authenticationService.Logout();

        [HttpPost]
        [Route(template: "register")]
        [AllowAnonymous]
        public async Task<UserViewModel> Register([FromBody] UserViewModel user)
            => await _authenticationService.Register(user);

        [HttpPost]
        [Route(template: "update")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task Update([FromBody] UserViewModel user)
            => await _authenticationService.Update(user);

        [HttpPost]
        [Route(template: "update/password")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task UpdatePassword([FromBody] UserViewModel user, string oldPassword)
            => await _authenticationService.UpdatePassword(user, oldPassword);
    }
}
