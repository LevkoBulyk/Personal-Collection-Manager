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
            await _userManager.DeleteAsync(user);
            if (Id.Equals(currectUserId))
            {
                await _signInManager.SignOutAsync();
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ConConfirmEmail(string token, string email)
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
    }
}
