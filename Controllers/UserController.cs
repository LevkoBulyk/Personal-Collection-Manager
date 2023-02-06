using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Personal_Collection_Manager.Data;
using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.Controllers
{
    public class UserController : Controller
    {
        private const string _success = "success";
        private const string _error = "error";
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserRepository _repository;

        public UserController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IUserRepository repository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _repository = repository;
        }

        public async Task<IActionResult> Index()
        {
            var users = _repository.GetAllNotDeletedUsers();
            // TODO: swithc to mapper
            var respond = new List<UserViewModel>();
            foreach (var user in users)
            {
                respond.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault(),
                    Blocked = user.Blocked
                });
            }
            return View(respond);
        }

        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            if (token == null || email == null)
                return BadRequest();
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return RedirectToAction("Error", "Home", $"No user found by email: {email}");
            var res = await _userManager.ConfirmEmailAsync(user, token);
            if (!res.Succeeded)
            {
                return RedirectToAction("Error", "Home", $"Failed to verify email: {email}");
            }
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ChangeRole(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return BadRequest();
            var currectUserId = (await _userManager.GetUserAsync(User)).Id;
            if (id.Equals(currectUserId))
            {
                await _signInManager.SignOutAsync();
                TempData[_success] = "You have succsessfully removed yourself from admin role";
                return RedirectToAction("Index", "Home");
            }
            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            await _userManager.RemoveFromRoleAsync(user, role);
            await _userManager.AddToRoleAsync(user, role.Equals(UserRole.Admin) ? UserRole.User : UserRole.Admin);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return BadRequest();
            var currectUserId = (await _userManager.GetUserAsync(User)).Id;
            if (id.Equals(currectUserId))
            {
                await _signInManager.SignOutAsync();
            }
            _repository.Delete(user.Id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Block(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return BadRequest();
            var currectUserId = (await _userManager.GetUserAsync(User)).Id;
            _repository.ReverceBlocked(id);
            if (id.Equals(currectUserId))
            {
                await _signInManager.SignOutAsync();
                TempData[_success] = "You have succsessfully blocked yourself";
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index");
        }

        public IActionResult Deleted()
        {
            var deletedUsers = _repository.GetAllDeletedUsers(User);
            return View(deletedUsers);
        }

        public IActionResult Restore(string id)
        {
            _repository.Restore(id);
            return RedirectToAction("Deleted");
        }
    }
}
