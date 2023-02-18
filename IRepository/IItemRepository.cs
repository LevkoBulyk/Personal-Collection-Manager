using Personal_Collection_Manager.Models;
using System.Security.Claims;

namespace Personal_Collection_Manager.IRepository
{
    public interface IItemRepository
    {
        public ItemViewModel GetItemById(int id);
        public ItemViewModel GetItemByIdAsNoTracking(int id);
        public ItemViewModel GetItemWithAdditionalFieldsOfCollection(int collectionId);
        public Task<bool> Create(ItemViewModel item);
        public Task<bool> Edit(ItemViewModel item);
        public Task<List<ItemListViewModel>> GetGetItemsForCollection(int collectionId);
    }
}
