using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.IService;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _repository;

        public ItemService(IItemRepository repository)
        {
            _repository = repository;
        }

        public ItemViewModel GetItemById(int? id)
        {
            ItemViewModel item;
            if (id == null)
            {
                item = new ItemViewModel();
            }
            else
            {
                item = _repository.GetItemById((int)id);
            }
            return item;
        }

        public ItemViewModel GetItemByIdAsNoTracking(int? id)
        {
            ItemViewModel item;
            if (id == null)
            {
                item = new ItemViewModel();
            }
            else
            {
                item = _repository.GetItemByIdAsNoTracking((int)id);
            }
            return item;
        }
    }
}
