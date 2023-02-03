using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Personal_Collection_Manager.Data;
using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.Controllers
{
    public class CollectionController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public CollectionController(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Create()
        {
            var collection = new CollectionCreate();
            var currentUserId = (await _userManager.GetUserAsync(User)).Id;
            collection.AuthorId = currentUserId;

            return View(collection);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CollectionCreate collection)
        {
            var col = new Collection();
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        public IActionResult AddField(CollectionCreate collection)
        {
            var count = collection.AdditionalFields.Length;
            if (count > 0)
            {
                var fields = collection.AdditionalFields;
                collection.AdditionalFields = new AditionalField[count + 1];
                for (int i = 0; i < count; i++)
                {
                    collection.AdditionalFields[i] = fields[i];
                }
            }
            else
            {
                collection.AdditionalFields = new AditionalField[1];
            }
            return View("Create", collection);
        }
    }
}
