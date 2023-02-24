using Personal_Collection_Manager.Data.DataBaseModels;
using System.Security.Claims;

namespace Personal_Collection_Manager.IRepository
{
    public interface IUserRepository
    {
        public List<ApplicationUser> GetAllNotDeletedUsers();
        public List<ApplicationUser> GetAllDeletedUsers(ClaimsPrincipal user);
        public void ReverceBlocked(string id);
        public bool Delete(string id);
        public bool Restore(string id);
        public Task<(string Id, string Email)> GetAuthorOfCollection(int collectionId);
    }
}
