using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Personal_Collection_Manager.Data;
using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.Models;
using System.Drawing;

namespace Personal_Collection_Manager.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ICollectionRepository _collection;

        public DashboardController(ICollectionRepository collection)
        {
            _collection = collection;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _collection.GetCollectionsOf(User));
        }
    }
}
