using CloudinaryDotNet;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.IService
{
    public interface IItemService
    {
        public Task<ItemViewModel> GetItemByIdAsNoTracking(int? id, int? collectionId, bool convertMarkdown = false);
        public Task<bool> Create(ItemViewModel item);
        public Task<bool> Edit(ItemViewModel item);
        public Task<bool> Delete(int id);
        public (bool Succeded, string Message) AddTag(ref ItemViewModel item);
        public (bool Succeded, string Message) RemoveTag(ref ItemViewModel item, int index);
        public Task<List<ItemListViewModel>> GetItemsOfCollection(int collectionId, int start = 0, int length = 10, string search = "");
        public Task<List<ItemNoFieldsViewModel>> GetRecentItems(int start = 0, int length = 10);
        public Task<List<ItemListViewModel>> GetAllItemsOfCollection(int collectionId);
        public IQueryable<ItemListViewModel> GetAllItemsWithTag(string tag);
        public IQueryable<ItemListViewModel> GetAllItemsOfCollectionAsQuery(int collectionId);
        public IQueryable<ItemNoFieldsViewModel> GetRecentItemsAsQuery(int start = 0, int length = 10);
    }
}
