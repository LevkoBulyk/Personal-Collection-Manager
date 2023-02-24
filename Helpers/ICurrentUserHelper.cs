using System.Security.Claims;

namespace Personal_Collection_Manager.Helpers
{
    public interface ICurrentUserHelper
    {
        public Task<bool> IsSignedIn(ClaimsPrincipal user);
        public Task<bool> IsSignedInAdmin(ClaimsPrincipal user);
        public Task<bool> HasEmail(string email, ClaimsPrincipal currentUser);
        public Task<bool> HasId(string id, ClaimsPrincipal currentUser);
        public Task<bool> HasEmailOrIsAdmin(string email, ClaimsPrincipal currentUser);
        public Task<bool> HasIdOrIsAdmin(string id, ClaimsPrincipal currentUser);
    }
}
