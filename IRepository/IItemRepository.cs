using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.IRepository
{
    public interface IItemRepository
    {
        public Task<ItemViewModel> GetItemByIdAsNoTracking(int id);
        public Task<ItemViewModel> GetItemWithAdditionalFieldsOfCollection(int collectionId);
        public Task<int> Create(ItemViewModel item);
        public Task<int> Edit(ItemViewModel item);
        public Task<int> Delete(int id);
        public Task<List<ItemListViewModel>> GetItemsOfCollection(int collectionId, int start, int length, string search);
        public Task<List<ItemNoFieldsViewModel>> GetRecentItems(int start, int length);
        public IQueryable<ItemNoFieldsViewModel> GetRecentItemsAsQuery(int start, int length);
        public Task<List<ItemListViewModel>> GetAllItemsOfCollection(int collectionId);
        public IQueryable<ItemListViewModel> GetAllItemsOfCollectionAsQuery(int collectionId);
    }
}
