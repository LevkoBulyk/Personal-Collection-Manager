using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.IService;

namespace Personal_Collection_Manager.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ICollectionService _collectionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(
            ICollectionService collectionService,
            UserManager<ApplicationUser> userManager)
        {
            _collectionService = collectionService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? userId)
        {
            if (userId == null)
            {
                return View(
                    (await _collectionService.GetCollectionsOf(User),
                    (await _userManager.GetUserAsync(User)).Id)
                    );
            }
            return View((await _collectionService.GetCollectionsOf(userId), userId));
        }
    }
}
