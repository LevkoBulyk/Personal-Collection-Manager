using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Personal_Collection_Manager.IService;
using Personal_Collection_Manager.Models;
using System.Linq.Expressions;

namespace Personal_Collection_Manager.Controllers
{
    public class ItemController : Controller
    {
        private const string _success = "success";
        private const string _error = "error";
        private readonly IItemService _itemService;
        private readonly ILikeService _likeService;

        public ItemController(
            IItemService itemService,
            ILikeService likeService)
        {
            _itemService = itemService;
            _likeService = likeService;
        }

        [Authorize]
        public async Task<IActionResult> Edit(int? id, int collectionId)
        {
            var item = await _itemService.GetItemByIdAsNoTracking(id, collectionId);
            return View(item);
        }

        [Authorize]
        public IActionResult AddTag(ItemViewModel item)
        {
            _itemService.AddTag(ref item);
            return View("Edit", item);
        }

        [HttpPost]
        [Authorize]
        public IActionResult RemoveTag(ItemViewModel item, int index)
        {
            _itemService.RemoveTag(ref item, index);
            return View("Edit", item);
        }

        [HttpGet]
        public IActionResult GetItemActions(string userId, int id)
        {
            return PartialView("_ItemActionsPartial", (userId, id));
        }

        [HttpPost]
        public async Task<IActionResult> GetAllItems(int collectionId)
        {
            var items = await _itemService.GetAllItemsOfCollectionAsQuery(collectionId).ToListAsync();
            return Json(items);
        }

        [HttpPost]
        public async Task<JsonResult> _GetAllItems(int collectionId)
        {
            int totalRecord = 0;
            int filterRecord = 0;
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");
            var data = _itemService.GetAllItemsOfCollectionAsQuery(collectionId);
            //get total count of data in table
            totalRecord = data.Count();
            // search data when search value found
            if (!string.IsNullOrEmpty(searchValue))
            {
                data = data.Where(item => item.Title.ToLower().Contains(searchValue.ToLower()) ||
                                  item.Tags.Any(tag => tag.ToLower().Contains(searchValue.ToLower())) ||
                                  item.Values.Any(value => value.ToLower().Contains(searchValue.ToLower())));
            }
            // get total count of records after search
            filterRecord = data.Count();
            //sort data
            // This part doesn't work due to the:
            // 1. It is not possible to get to the spesific element of Values property of the ItemListViewModel
            //      possible solution:
            //          usage of the relfection to built the ItemListViewModel for each collection with all additional fields
            // Additional problems: 
            //      each of the Items in IQueriable<ItemListViewModel> contain Tags and Values, which already were taken from the DB
            if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDirection))
            {
                var parameter = Expression.Parameter(typeof(ItemListViewModel), "x");
                MemberExpression? property;
                if (sortColumn != "Title")
                {
                    var index = int.Parse(sortColumn);
                    property = Expression.Property(Expression.Property(parameter, "Values"), $"ElementAtOrDefault({index})");
                    //property = Expression.Property(parameter, $"Values[{sortColumn}]");
                }
                else
                {
                    property = Expression.Property(parameter, sortColumn);
                }
                var lambda = Expression.Lambda(property, parameter);
                var method = string.Equals(sortColumnDirection, "desc", StringComparison.OrdinalIgnoreCase)
                    ? "OrderByDescending"
                    : "OrderBy";
                var expression = Expression.Call(typeof(Queryable), method, new[] { typeof(ItemListViewModel), property.Type }, data.Expression, Expression.Quote(lambda));
                data = data.Provider.CreateQuery<ItemListViewModel>(expression);
            }
            //pagination
            var empList = data.Skip(skip).Take(pageSize).ToList();
            var returnObj = new
            {
                draw = draw,
                recordsTotal = totalRecord,
                recordsFiltered = filterRecord,
                data = empList
            };

            return Json(returnObj);

            /*
            var items = await _itemService.GetAllItemsOfCollection(collectionId);
            return Json(items);*/
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(ItemViewModel item)
        {
            ModelState.Remove("Id");
            ModelState.Remove("AuthorId");
            ModelState.Remove("AuthorEmail");
            if (!ModelState.IsValid)
            {
                TempData[_error] = "Not all required fields were filled, or some were filled with errors";
                return View(item);
            }
            if (item.Id == null)
            {
                if (await _itemService.Create(item))
                {
                    TempData[_success] = "New item was successfully created";
                    return RedirectToAction("Details", "Collection", new { id = item.CollectionId });

                }
            }
            else
            {
                if (await _itemService.Edit(item))
                {
                    TempData[_success] = "Item was saved";
                    return RedirectToAction("Details", "Collection", new { id = item.CollectionId });
                }
            }
            TempData[_error] = "Item was not saved! Check your input! If it won't help, please contact admin, cause we are having serious problems";
            return View(item);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var item = await _itemService.GetItemByIdAsNoTracking(id, null, true);
            return View(item);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            int collectionId = (await _itemService.GetItemByIdAsNoTracking(id, null)).CollectionId;
            await _itemService.Delete(id);
            if (collectionId != null)
            {
                return RedirectToAction("Details", "Collection", new { id = collectionId });
            }
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ThumbUp(int itemId, string userId)
        {
            await _likeService.ThumbUp(itemId, userId);
            var newQuantityOfLikes = await _likeService.CountLikesOfItemAsync(itemId);
            return Json(newQuantityOfLikes);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ThumbDown(int itemId, string userId)
        {
            await _likeService.ThumbDown(itemId, userId);
            var newQuantityOfLikes = await _likeService.CountLikesOfItemAsync(itemId);
            return Json(newQuantityOfLikes);
        }

        [Authorize]
        public async Task<IActionResult> FindLike(int itemId, string userId)
        {
            var like = await _likeService.FindLike(itemId, userId);
            if (like == null)
            {
                return Json(null);
            }
            else
            {
                return Json(like.ThumbUp);
            }
        }
    }
}
