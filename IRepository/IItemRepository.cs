using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.IRepository
{
    public interface IItemRepository
    {
        public Task<ItemViewModel> GetItemByIdAsNoTracking(int id);
        public Task<ItemViewModel> GetItemWithAdditionalFieldsOfCollection(int collectionId);
        public Task<bool> Create(ItemViewModel item);
        public Task<bool> Edit(ItemViewModel item);
        public Task<List<ItemListViewModel>> GetItemsOfCollection(int collectionId);
    }
}
