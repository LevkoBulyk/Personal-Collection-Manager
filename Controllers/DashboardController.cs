using Microsoft.AspNetCore.Mvc;
using Personal_Collection_Manager.IService;

namespace Personal_Collection_Manager.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ICollectionService _collection;

        public DashboardController(ICollectionService collection)
        {
            _collection = collection;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _collection.GetCollectionsOf(User));
        }
    }
}
