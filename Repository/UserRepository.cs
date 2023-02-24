using Microsoft.AspNetCore.Identity;
using Personal_Collection_Manager.Data;
using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.IRepository;
using System.Security.Claims;

namespace Personal_Collection_Manager.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        void IUserRepository.ReverceBlocked(string id)
        {
            var user = _dbContext.Users.Find(id);
            user.Blocked = !user.Blocked;
            _dbContext.SaveChanges();
        }

        public bool Delete(string id)
        {
            var userToDelete = _dbContext.Users.FirstOrDefault(u => u.Id.Equals(id));
            userToDelete.Deleted = true;
            return _dbContext.SaveChanges() > 0;
        }

        public List<ApplicationUser> GetAllNotDeletedUsers()
        {
            var users = (from u in _dbContext.Users
                         where !u.Deleted
                         select u).ToList();
            return users;
        }

        public List<ApplicationUser> GetAllDeletedUsers(ClaimsPrincipal user)
        {
            var users = (from u in _dbContext.Users
                         where u.Deleted
                         select u).ToList();
            return users;
        }

        public bool Restore(string id)
        {
            var userToDelete = _dbContext.Users.FirstOrDefault(u => u.Id.Equals(id));
            userToDelete.Deleted = false;
            return _dbContext.SaveChanges() > 0;
        }

        public async Task<(string Id, string Email)> GetAuthorOfCollection(int collectionId)
        {
            var id = (await _dbContext.Collections.FindAsync(collectionId)).UserId;
            var email = (await _dbContext.Users.FindAsync(id)).Email;
            return (id, email);
        }
    }
}
