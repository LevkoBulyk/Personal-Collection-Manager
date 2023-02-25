using CloudinaryDotNet;
using Personal_Collection_Manager.Data.DataBaseModels.Enum;
using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.IService;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.Services
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _itemRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMarkdownService _markdownService;

        public ItemService(
            IItemRepository ItemRepository,
            IUserRepository userRepository,
            IMarkdownService markdownService)
        {
            _itemRepository = ItemRepository;
            _userRepository = userRepository;
            _markdownService = markdownService;
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
            return (await _itemRepository.Create(item)) > 0;
        }

        public async Task<bool> Delete(int id)
        {
            return (await _itemRepository.Delete(id)) > 0;
        }

        public async Task<bool> Edit(ItemViewModel item)
        {
            if (item.Id == null)
            {
                throw new ArgumentNullException("Item id can not be 'null'");
            }
            return (await _itemRepository.Edit(item)) > 0;
        }

        public async Task<ItemViewModel> GetItemByIdAsNoTracking(int? id, int? collectionId, bool convertMarkdown)
        {
            ItemViewModel item;
            if (id == null)
            {
                item = await _itemRepository.GetItemWithAdditionalFieldsOfCollection((int)collectionId);
            }
            else
            {
                item = await _itemRepository.GetItemByIdAsNoTracking((int)id);
            }
            var author = await _userRepository.GetAuthorOfCollection(item.CollectionId);
            item.AuthorId = author.Id;
            item.AuthorEmail = author.Email;
            if (convertMarkdown)
                foreach (var field in item.Fields)
                {
                    if (field.Type == FieldType.MultyLineString)
                    {
                        field.Value = _markdownService.ToHtml(field.Value);
                    }
                }
            return item;
        }

        public Task<List<ItemListViewModel>> GetItemsOfCollection(int collectionId, int start, int length, string search)
        {
            return _itemRepository.GetItemsOfCollection(collectionId, start, length, search);
        }

        public Task<List<ItemListViewModel>> GetAllItemsOfCollection(int collectionId)
        {
            return _itemRepository.GetAllItemsOfCollection(collectionId);
        }

        public IQueryable<ItemListViewModel> GetAllItemsOfCollectionAsQuery(int collectionId)
        {
            return _itemRepository.GetAllItemsOfCollectionAsQuery(collectionId);
        }
    }
}
