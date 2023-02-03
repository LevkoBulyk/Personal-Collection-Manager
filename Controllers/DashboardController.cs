using Microsoft.AspNetCore.Mvc;
using Personal_Collection_Manager.Data;
using Personal_Collection_Manager.Models;
using System.Drawing;

namespace Personal_Collection_Manager.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string Id)
        {
            var collections = (from coll in _context.Collections
                               where coll.UserId.Equals(Id)
                               select new CollectionView() {
                                   Id = coll.Id,
                                   Name = coll.Name,
                                   Description = coll.Description,
                                   Topic = coll.Topic,
                                   ImageUrl = coll.ImageUrl
                               }).ToList();
            return View(collections);
        }
    }
}
