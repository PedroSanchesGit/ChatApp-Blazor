using ChatApp.Models;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace ChatApp.Services
{
    public class LoginService
    {


        #region Variables
        private User? user;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        #endregion

        #region Constructor
        public LoginService(AuthenticationStateProvider authenticationStateProvider)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }
        #endregion

        #region Properties
        public User? User { get { return user; } set { user = value; } }

        #endregion

        #region Methods

        /// <summary>
        /// Method that retrieves and returns the current Authentication State
        /// </summary>
        /// <returns>returns the current Authentication State</returns>
        public async Task<AuthenticationState> AuthInfo()
        {
            return await _authenticationStateProvider.GetAuthenticationStateAsync();
        }

        /// <summary>
        /// Checks if there's a User logged in
        /// </summary>
        /// <returns>boolean value</returns>
        public async Task<bool> IsLoggedIn()
        {
            if ((await AuthInfo()).User != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Method that gets all the user necessary values upon loggin in through SSO and stores all that on a Singleton
        /// </summary>
        /// <param name="Configuration">Varable that holds the configuration values</param>
        /// <returns>New User</returns>
        public async Task<User?> Login(IConfiguration Configuration)
        {
            ActiveDirectoryService adService = new ActiveDirectoryService(Configuration);

            var authState = (await AuthInfo());

            if (authState.User?.Identity?.IsAuthenticated == true)
            {
                string username = await adService.GetInfoByUserLoggedIn(authState.User?.Identity?.Name, "samaccountname");
                string title = await adService.GetInfoByUserLoggedIn(authState.User?.Identity?.Name, "title");
                string name = await adService.GetInfoByUserLoggedIn(authState.User?.Identity?.Name, "displayname");
                string principalName = await adService.GetInfoByUserLoggedIn(authState.User?.Identity?.Name, "msds-principalname");
                string email = await adService.GetInfoByUserLoggedIn(authState.User?.Identity?.Name, "email");
                string photo = await adService.GetInfoByUserLoggedIn(authState.User?.Identity?.Name, "thumbnailphoto");

                List<string> roles = adService.ListOuFromUser(authState.User?.Identity?.Name);

                User userLoggedIn = new User() { Username = username, Photo = photo, Title = title, DisplayName = name, PrincipalName = principalName, Email = email, Role = roles };

                this.User = userLoggedIn;

                CreateIdentityFromUser(userLoggedIn);

                return this.User;

            }

            return null;
        }

        /// <summary>
        /// Method that creates a ClaimsIdentity related to the user's roles in order to seggregate the available areas of the software
        /// </summary>
        /// <param name="user">User in question</param>
        /// <returns></returns>
        private ClaimsIdentity CreateIdentityFromUser(User user)
        {
            var result = new ClaimsIdentity(new Claim[]
            {
                new (ClaimTypes.Name, user.Username)
            });

            foreach (string role in user.Role)
            {
                result.AddClaim(new(ClaimTypes.Role, role));
            }

            return result;
        }

        #endregion
    }
}