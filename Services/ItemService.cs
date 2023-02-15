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

        public Task<bool> Create(ItemViewModel item)
        {
            return _repository.Create(item);
        }

        public bool Edit(ItemViewModel item)
        {
            throw new NotImplementedException();
        }

        public ItemViewModel GetItemById(int? id, int? collectionId)
        {
            ItemViewModel item;
            if (id == null)
            {
                item = _repository.GetItemWithAdditionalFieldsOfCollection((int)collectionId);
            }
            else
            {
                item = _repository.GetItemById((int)id);
            }
            return item;
        }

        public ItemViewModel GetItemByIdAsNoTracking(int? id, int? collectionId)
        {
            ItemViewModel item;
            if (id == null)
            {
                item = _repository.GetItemWithAdditionalFieldsOfCollection((int)collectionId);
            }
            else
            {
                item = _repository.GetItemByIdAsNoTracking((int)id);
            }
            return item;
        }
    }
}
