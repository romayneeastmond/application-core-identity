using ApplicationCoreIdentity.Services.ViewModels;
using System.Threading.Tasks;

namespace ApplicationCoreIdentity.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<UserViewModel> Login(string emailAddress, string password, bool rememberMe);

        void Logout();

        Task<UserViewModel> Register(UserViewModel user);

        Task Update(UserViewModel user);

        Task UpdatePassword(UserViewModel user, string oldPassword);
    }
}
