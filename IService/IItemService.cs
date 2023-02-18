using Personal_Collection_Manager.Models;
using System.Security.Claims;

namespace Personal_Collection_Manager.IService
{
    public interface IItemService
    {
        public Task<ItemViewModel> GetItemByIdAsNoTracking(int? id, int? collectionId);
        public Task<bool> Create(ItemViewModel item);
        public Task<bool> Edit(ItemViewModel item);
        public (bool Succeded, string Message) AddTag(ref ItemViewModel item);
        public Task<List<ItemListViewModel>> GetItemsForCollection(int collectionId);
    }
}
