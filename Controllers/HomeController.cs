using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Personal_Collection_Manager.IService;
using Personal_Collection_Manager.Models;
using System.Diagnostics;
using System.Drawing;
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

        [HttpPost]
        public async Task<JsonResult> GetResentItems()
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
        }

        [HttpPost]
        public IActionResult SetTheme(string theme)
        {
            var cookie = new CookieOptions();
            cookie.Expires = DateTime.Now.AddMonths(1);
            Response.Cookies.Append("theme", theme, cookie);
            return Ok();
        }

        [HttpPost]
        public IActionResult SetCulture(string culture)
        {
            var cookie = new CookieOptions();
            cookie.Expires = DateTime.Now.AddMonths(1);
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                cookie);
            return Ok();
        }
    }
}