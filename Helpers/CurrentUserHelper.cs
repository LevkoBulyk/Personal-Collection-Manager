using Microsoft.AspNetCore.Identity;
using Personal_Collection_Manager.Data.DataBaseModels;
using System.Security.Claims;

namespace Personal_Collection_Manager.Helpers
{
    public class CurrentUserHelper : ICurrentUserHelper
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public CurrentUserHelper(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> HasEmail(string email, ClaimsPrincipal currentUser)
        {
            return GetEmail(currentUser).Result.Equals(email);
        }

        public async Task<bool> HasEmailOrIsAdmin(string email, ClaimsPrincipal currentUser)
        {
            return await HasEmail(email, currentUser) || await IsSignedInAdmin(currentUser);
        }

        public async Task<bool> HasId(string id, ClaimsPrincipal currentUser)
        {
            return GetId(currentUser).Result.Equals(id);
        }

        public async Task<bool> HasIdOrIsAdmin(string id, ClaimsPrincipal currentUser)
        {
            return await HasId(id, currentUser) || await IsSignedInAdmin(currentUser);
        }

        public async Task<bool> IsSignedIn(ClaimsPrincipal user)
        {
            var u = await _userManager.GetUserAsync(user);
            return u != null;
        }

        public async Task<bool> IsSignedInAdmin(ClaimsPrincipal user)
        {
            var u = await _userManager.GetUserAsync(user);
            return u != null ? await _userManager.IsInRoleAsync(u, UserRole.Admin) : false;
        }

        private async Task<string> GetEmail(ClaimsPrincipal user)
        {
            var u = await _userManager.GetUserAsync(user);
            return u != null ? u.Email : "null";
        }

        private async Task<string> GetId(ClaimsPrincipal user)
        {
            var u = await _userManager.GetUserAsync(user);
            return u != null ? u.Id : "null";
        }
    }
}
