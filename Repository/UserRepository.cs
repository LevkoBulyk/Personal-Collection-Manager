using Microsoft.AspNetCore.Identity;
using Personal_Collection_Manager.Data;
using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.IRepository;
using System.Security.Claims;

namespace Personal_Collection_Manager.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        void IUserRepository.ReverceBlocked(string id)
        {
            var user = _context.Users.Find(id);
            user.Blocked = !user.Blocked;
            _context.SaveChanges();
        }

        public bool Delete(string id)
        {
            var userToDelete = _context.Users.FirstOrDefault(u => u.Id.Equals(id));
            userToDelete.Deleted = true;
            return _context.SaveChanges() > 0;
        }

        public List<ApplicationUser> GetAllNotDeletedUsers()
        {
            var users = (from u in _context.Users
                         where !u.Deleted
                         select u).ToList();
            return users;
        }

        public List<ApplicationUser> GetAllDeletedUsers(ClaimsPrincipal user)
        {
            if (!user.IsInRole(UserRole.Admin))
            {
                throw new UnauthorizedAccessException();
            }
            var users = (from u in _context.Users
                        where u.Deleted
                        select u).ToList();
            return users;
        }

        public bool Restore(string id)
        {
            var userToDelete = _context.Users.FirstOrDefault(u => u.Id.Equals(id));
            userToDelete.Deleted = false;
            return _context.SaveChanges() > 0;
        }
    }
}
