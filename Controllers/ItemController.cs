using Microsoft.AspNetCore.Mvc;
using Personal_Collection_Manager.IService;

namespace Personal_Collection_Manager.Controllers
{
    public class ItemController : Controller
    {
        private readonly IItemService _itemService;

        public ItemController(IItemService itemService)
        {
            _itemService = itemService;
        }

        public IActionResult Edit(int? id, int collectionId)
        {
            var item = _itemService.GetItemByIdAsNoTracking(id);
            item.CollectionId = collectionId;
            return View(item);
        }
    }
}
