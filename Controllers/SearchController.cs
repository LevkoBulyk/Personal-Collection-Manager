using Microsoft.AspNetCore.Mvc;
using Personal_Collection_Manager.IService;

namespace Personal_Collection_Manager.Controllers
{
    public class SearchController : Controller
    {
        private readonly IItemService _itemService;

        public SearchController(IItemService itemService)
        {
            _itemService = itemService;
        }

        public IActionResult Tags(string text)
        {
            return View();
        }

        public IActionResult Index(string tag)
        {
            return View("Index", tag);
        }

        public async Task<IActionResult> GetItems(string tag)
        {
            int totalRecord = 0;
            int filterRecord = 0;
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var data = _itemService.GetAllItemsWithTag(tag);
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
    }
}
