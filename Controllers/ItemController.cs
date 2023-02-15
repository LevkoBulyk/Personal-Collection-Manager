using Microsoft.AspNetCore.Mvc;
using Personal_Collection_Manager.IService;
using Personal_Collection_Manager.Models;
using Personal_Collection_Manager.Repository.Exceptions;

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

        public IActionResult Edit(int? id, int collectionId)
        {
            var item = _itemService.GetItemByIdAsNoTracking(id, collectionId);
            return View(item);
        }

        [HttpPost]
        public IActionResult AddTag(ItemViewModel item)
        {
            _itemService.AddTag(ref item);
            return View("Edit", item);
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
            /*try
            {*/
            if (item.Id == null)
            {
                if (await _itemService.Create(item))
                {
                    TempData[_success] = "New item was successfully created";
                    return RedirectToAction("Detail", "Collection", new { id = item.CollectionId });

                }
            }
            else
            {
                if (_itemService.Edit(item))
                {
                    TempData[_success] = "Item was saved";
                    return RedirectToAction("Detail", "Collection", new { id = item.CollectionId });
                }
            }
            /*}
            catch (TopicNotFoundException e)
            {
                TempData[_error] = e.Message;
                return View(item);
            }*/
            TempData[_error] = "Item was not saved! Check your input! If it won't help, please contact admin, cause we are having serious problems";
            return View(item);
        }
    }
}
