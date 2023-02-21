using Microsoft.AspNetCore.Mvc;
using Personal_Collection_Manager.IService;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.Controllers
{
    public class ItemController : Controller
    {
        private const string _success = "success";
        private const string _error = "error";
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        public async Task<IActionResult> Edit(int? id, int collectionId)
        {
            var item = await _itemService.GetItemByIdAsNoTracking(id, collectionId);
            return View(item);
        }

        public IActionResult AddTag(ItemViewModel item)
        {
            _itemService.AddTag(ref item);
            return View("Edit", item);
        }

        [HttpGet]
        public async Task<IActionResult> GetItemsList(int collectionId, int pageNumber)
        {
            var items = await _itemService.GetItemsForCollection(collectionId, pageNumber);
            return PartialView("_ItemListPartial", items);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(ItemViewModel item)
        {
            ModelState.Remove("Id");
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
            var item = await _itemService.GetItemByIdAsNoTracking(id, null);
            return View(item);
        }

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
    }
}
