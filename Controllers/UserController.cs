using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
