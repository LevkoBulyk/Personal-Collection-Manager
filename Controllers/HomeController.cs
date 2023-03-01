using Microsoft.AspNetCore.Mvc;
using Personal_Collection_Manager.IService;
using Personal_Collection_Manager.Models;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Personal_Collection_Manager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICollectionService _collectionService;
        private readonly IItemService _itemService;

        public HomeController(
            ILogger<HomeController> logger,
            ICollectionService collectionService,
            IItemService itemService)
        {
            _logger = logger;
            _collectionService = collectionService;
            _itemService = itemService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetBiggestCollections()
        {
            var collections = await _collectionService.GetTheBiggestCollections();
            return PartialView("/Views/Collection/_CollectionsPartial.cshtml", collections);
        }

        [HttpGet]
        public async Task<IActionResult> GetRecentItems()
        {
            var items = await _itemService.GetRecentItems();
            return Json(items);
        }

        [HttpPost]
        public async Task<JsonResult> HereGetResentItems()
        {
            int totalRecord = 0;
            int filterRecord = 0;
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var data = _itemService.GetRecentItemsAsQuery();
            //get total count of data in table
            totalRecord = data.Count();
            // get total count of records after search
            filterRecord = totalRecord;
            //pagination
            var itemsList = data.Skip(skip).Take(pageSize).ToList();
            var returnObj = new
            {
                draw = draw,
                recordsTotal = totalRecord,
                recordsFiltered = filterRecord,
                data = itemsList
            };

            return Json(returnObj);

            /*
            var items = await _itemService.GetAllItemsOfCollection(collectionId);
            return Json(items);*/
        }

        [HttpPost]
        public IActionResult SetTheme(string theme)
        {
            var cookie = new CookieOptions();
            cookie.Expires = DateTime.Now.AddMonths(1);
            Response.Cookies.Append("theme", theme, cookie);
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}