using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public UserController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = _userManager.Users.Take(10).ToList();
            // TODO: swithc to mapper
            var respond = new List<UserView>();
            foreach (var user in users)
            {
                respond.Add(new UserView { 
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Role = _userManager.GetRolesAsync(user).Result.FirstOrDefault()
            });
            }
            return View(respond);
        }

        public async Task<IActionResult> ChangeRole(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
                return BadRequest();
            var currectUserId = (await _userManager.GetUserAsync(User)).Id;
            if (Id.Equals(currectUserId))
            {
                await _signInManager.SignOutAsync();
            }
            var role = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            await _userManager.RemoveFromRoleAsync(user, role);
            await _userManager.AddToRoleAsync(user, role.Equals(UserRole.Admin)?UserRole.User:UserRole.Admin);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string Id)
        {
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
                return BadRequest();
            var currectUserId = (await _userManager.GetUserAsync(User)).Id;
            if (Id.Equals(currectUserId))
            {
                await _signInManager.SignOutAsync();
            }
            await _userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Block(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return BadRequest();
            // TODO: user blocking
            return RedirectToAction("Index");
        }
    }
}
