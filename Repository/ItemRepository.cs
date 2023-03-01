using Microsoft.EntityFrameworkCore;
using Personal_Collection_Manager.Data;
using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.Repository
{
    public class ItemRepository : IItemRepository
    {
        private const int pageSize = 10;
        private readonly ApplicationDbContext _dbContext;
        private readonly ITagRepository _tagRepository;
        private readonly IFieldOfItemRepository _fieldOfItemRepository;

        public ItemRepository(
            ApplicationDbContext dbContext,
            ITagRepository tagRepository,
            IFieldOfItemRepository fieldOfItemRepository)
        {
            _dbContext = dbContext;
            _tagRepository = tagRepository;
            _fieldOfItemRepository = fieldOfItemRepository;
        }

        public async Task<ItemViewModel> GetItemWithAdditionalFieldsOfCollection(int collectionId)
        {
            var fields = await (_dbContext.AdditionalFieldsOfCollections
                .Where(af => af.CollectionId == collectionId && !af.Deleted)
                .OrderBy(af => af.Order)
                .Select(af => new ItemField()
                {
                    Order = af.Order,
                    Title = af.Title,
                    Type = af.Type,
                    AdditionalFieldOfCollectionId = af.Id,
                })).AsNoTracking().ToArrayAsync();
            return new ItemViewModel()
            {
                CollectionId = collectionId,
                Fields = fields
            };
        }

        public async Task<ItemViewModel> GetItemByIdAsNoTracking(int id)
        {
            var item = _dbContext.Items
                .Where(i => i.Id == id && !i.Deleted)
                .Select(i => new ItemViewModel()
                {
                    Id = i.Id,
                    CollectionId = i.CollectionId,
                    Title = i.Title,
                }).AsNoTracking().SingleOrDefault();
            item.Fields = await _fieldOfItemRepository.GetFieldsOfItemAsNoTraking(id);
            item.Tags = (from tag in _tagRepository.GetTagsOfItem(id)
                         select tag.Value).ToArray();
            return item;
        }

        public async Task<int> Create(ItemViewModel item)
        {
            var itemToAdd = new Item()
            {
                Title = item.Title,
                CollectionId = item.CollectionId
            };
            await _dbContext.Items.AddAsync(itemToAdd);
            var res = _dbContext.SaveChanges();
            res += await _tagRepository.AddTagsToItem((int)itemToAdd.Id, item.Tags.ToList());
            res += await _fieldOfItemRepository.CreateFieldsForItem(itemToAdd.Id, item.Fields);
            return res;
        }

        public async Task<int> Edit(ItemViewModel item)
        {
            var itemToEdit = await _dbContext.Items.FindAsync(item.Id);
            itemToEdit.Title = item.Title;
            _dbContext.Items.Update(itemToEdit);
            var res = _dbContext.SaveChanges();
            res += await _tagRepository.RemoveTagsOfItem(itemToEdit.Id);
            res += await _tagRepository.AddTagsToItem(itemToEdit.Id, item.Tags.ToList());
            res += await _fieldOfItemRepository.UpdateFieldsForItem((int)item.Id, item.Fields);
            return res;
        }

        public async Task<int> Delete(int id)
        {
            var itemToDelete = await (from item in _dbContext.Items
                                      where item.Id == id
                                      select item).SingleOrDefaultAsync();
            if (itemToDelete != null)
            {
                itemToDelete.Deleted = true;
                _dbContext.Items.Update(itemToDelete);
            }
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<List<ItemListViewModel>> GetItemsOfCollection(int collectionId, int start, int length, string search)
        {
            return await GetAllItemsOfCollectionAsQuery(collectionId).Skip(start).Take(length)
                .AsNoTracking().ToListAsync();
        }

        public async Task<List<ItemListViewModel>> GetAllItemsOfCollection(int collectionId)
        {
            return await GetAllItemsOfCollectionAsQuery(collectionId)
                .AsNoTracking().ToListAsync();
        }

        public IQueryable<ItemListViewModel> GetAllItemsOfCollectionAsQuery(int collectionId)
        {
            return _dbContext.Items
                .Where(item => item.CollectionId == collectionId && !item.Deleted)
                .OrderByDescending(item => item.Id)
                .Select(item => new ItemListViewModel()
                {
                    Id = item.Id,
                    Title = item.Title,
                    Tags = _dbContext.ItemsTags
                        .Where(itemTags => itemTags.ItemId == item.Id)
                        .Join(_dbContext.Tags,
                            itemTags => itemTags.TagId,
                            tag => tag.Id,
                            (itemTag, tag) => tag)
                        .Select(tag => tag.Value).ToList(),
                    Values = _dbContext.AdditionalFieldsOfCollections
                        .Where(afoc => afoc.CollectionId == collectionId && !afoc.Deleted)
                        .GroupJoin(_dbContext.FieldsOfItems
                                .Where(foi => foi.ItemId == item.Id),
                            afoc => afoc.Id,
                            foi => foi.AdditionalFieldOfCollectionId,
                            (afoc, fois) => new { AdditionalFieldOfCollection = afoc, FieldsOfItems = fois })
                        .SelectMany(x => x.FieldsOfItems.DefaultIfEmpty(),
                            (afoc, foi) => new
                            {
                                Value = foi != null ? foi.Value : "",
                                Order = afoc.AdditionalFieldOfCollection.Order
                            })
                        .OrderBy(x => x.Order)
                        .Select(x => x.Value).ToList(),
                    UserId = _dbContext.Collections
                        .Where(collection => collection.Id == collectionId)
                        .Single().UserId
                });
        }

        public IQueryable<ItemNoFieldsViewModel> GetRecentItemsAsQuery(int start, int length)
        {
            return _dbContext.Items
                .Where(item => !item.Deleted)
                .OrderByDescending(item => item.Id)
                .Join(_dbContext.Collections,
                    item => item.CollectionId,
                    collection => collection.Id,
                    (item, collection) => new { Item = item, Collection = collection })
                .Join(_dbContext.Users,
                    x => x.Collection.UserId,
                    user => user.Id,
                    (x, user) => new { x.Item, x.Collection, User = user })
                .Select(x => new ItemNoFieldsViewModel()
                {
                    Id = x.Item.Id,
                    Title = x.Item.Title,
                    Tags = _dbContext.ItemsTags
                        .Where(itemTags => itemTags.ItemId == x.Item.Id)
                        .Join(_dbContext.Tags,
                            itemTags => itemTags.TagId,
                            tag => tag.Id,
                            (itemTag, tag) => tag)
                        .Select(tag => tag.Value).ToArray(),
                    AuthorId = x.User.Id,
                    AuthorEmail = x.User.Email,
                    CollectionTitle = x.Collection.Title,
                    CollectionId = x.Item.CollectionId
                });
        }

        public async Task<List<ItemNoFieldsViewModel>> GetRecentItems(int start, int length)
        {
            return await _dbContext.Items
                .Where(item => !item.Deleted)
                .OrderByDescending(item => item.Id)
                .Join(_dbContext.Collections,
                    item => item.CollectionId,
                    collection => collection.Id,
                    (item, collection) => new { Item = item, Collection = collection })
                .Join(_dbContext.Users,
                    x => x.Collection.UserId,
                    user => user.Id,
                    (x, user) => new { x.Item, x.Collection, User = user })
                .Select(x => new ItemNoFieldsViewModel()
                {
                    Id = x.Item.Id,
                    Title = x.Item.Title,
                    Tags = _dbContext.ItemsTags
                        .Where(itemTags => itemTags.ItemId == x.Item.Id)
                        .Join(_dbContext.Tags,
                            itemTags => itemTags.TagId,
                            tag => tag.Id,
                            (itemTag, tag) => tag)
                        .Select(tag => tag.Value).ToArray(),
                    AuthorId = x.User.Id,
                    AuthorEmail = x.User.Email,
                    CollectionTitle = x.Collection.Title,
                    CollectionId = x.Item.CollectionId
                }).Skip(start).Take(length).ToListAsync();
        }
    }
}
