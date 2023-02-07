using Microsoft.AspNetCore.Mvc;
using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.Controllers
{
    public class ItemController : Controller
    {
        private readonly IItemRepository _itemRepository;

        public ItemController(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        public IActionResult Edit(int? id)
        {
            ItemViewModel item;
            if (id == null)
            {
                item = new ItemViewModel();
            }
            else
            {
                
            }
            return View();
        }
    }
}
