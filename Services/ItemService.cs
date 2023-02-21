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

        public (bool Succeded, string Message) AddTag(ref ItemViewModel item)
        {
            var count = item.Tags.Length;
            if (count > 0)
            {
                var oldTags = item.Tags;
                item.Tags = new string[count + 1];
                item.Tags[count] = string.Empty;
                for (int i = 0; i < count; i++)
                {
                    item.Tags[i] = oldTags[i];
                }
            }
            else
            {
                item.Tags = new string[1];
            }
            return (
                Succeded: true,
                Message: "Tag added"
                );
        }

        public async Task<bool> Create(ItemViewModel item)
        {
            return (await _repository.Create(item)) > 0;
        }

        public async Task<bool> Delete(int id)
        {
            return (await _repository.Delete(id)) > 0;
        }

        public async Task<bool> Edit(ItemViewModel item)
        {
            if (item.Id == null)
            {
                throw new ArgumentNullException("Item id can not be 'null'");
            }
            return (await _repository.Edit(item)) > 0;
        }

        public async Task<ItemViewModel> GetItemByIdAsNoTracking(int? id, int? collectionId)
        {
            ItemViewModel item;
            if (id == null)
            {
                item = await _repository.GetItemWithAdditionalFieldsOfCollection((int)collectionId);
            }
            else
            {
                item = await _repository.GetItemByIdAsNoTracking((int)id);
            }
            return item;
        }

        public Task<List<ItemListViewModel>> GetAllItemsForCollection(int collectionId)
        {
            return _repository.GetAllItemsOfCollection(collectionId);
        }

        public Task<List<ItemListViewModel>> GetItemsForCollection(int collectionId, int page)
        {
            return _repository.GetItemsOfCollection(collectionId, page);
        }
    }
}
