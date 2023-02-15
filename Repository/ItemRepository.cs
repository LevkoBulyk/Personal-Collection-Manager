using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using Personal_Collection_Manager.Data;
using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.Repository
{
    public class ItemRepository : IItemRepository
    {
        private readonly ApplicationDbContext _context;

        public ItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ItemViewModel GetItemWithAdditionalFieldsOfCollection(int collectionId)
        {
            return new ItemViewModel()
            {
                CollectionId = collectionId,
                Fields = (_context.AdditionalFieldsOfCollections
                .Where(af => af.CollectionId == collectionId && !af.Deleted)
                .OrderBy(af => af.Order)
                .Select(af => new ItemField()
                {
                    Order = af.Order,
                    Title = af.Title,
                    Type = af.Type,
                    AdditionalFieldOfCollectionId = af.Id
                })).AsNoTracking().ToArray()
            };
        }

        public ItemViewModel GetItemById(int id)
        {
            var item = (from i in _context.Items
                        where i.Id == id && !i.Deleted
                        select new ItemViewModel()
                        {
                            Id = i.Id,
                            CollectionId = i.CollectionId,
                            Title = i.Title,
                            Tags = (from tagItem in _context.ItemsTags
                                    join tag in _context.Tags
                                    on tagItem.TagId equals tag.Id
                                    where tagItem.ItemId == id
                                    select tag.Value).ToArray(),
                            Fields = (_context.FieldsOfItems
                                .Where(field => field.ItemId == id)
                                .Join(_context.AdditionalFieldsOfCollections,
                                    field => field.AdditionalFieldOfCollectionId,
                                    collField => collField.Id,
                                    (field, collField) => new ItemField
                                    {
                                        Order = collField.Order,
                                        Title = collField.Title,
                                        Value = field.Value,
                                        Type = collField.Type,
                                        AdditionalFieldOfCollectionId = collField.Id
                                    })).ToArray()
                        }).SingleOrDefault();
            return item;
        }

        public ItemViewModel GetItemByIdAsNoTracking(int id)
        {
            var item = _context.Items
                .Where(i => i.Id == id && !i.Deleted)
                .Select(i => new ItemViewModel()
                {
                    Id = i.Id,
                    CollectionId = i.CollectionId,
                    Title = i.Title,
                    Tags = _context.ItemsTags
                        .Where(tagItem => tagItem.ItemId == id)
                        .Join(_context.Tags, tagItem => tagItem.TagId, tag => tag.Id, (tagItem, tag) => tag.Value)
                        .AsNoTracking().ToArray(),
                    Fields = _context.FieldsOfItems
                        .Join(_context.AdditionalFieldsOfCollections,
                            field => field.AdditionalFieldOfCollectionId,
                            collField => collField.Id,
                            (field, collField) => new ItemField()
                            {
                                Order = collField.Order,
                                Title = collField.Title,
                                Value = field.Value,
                                Type = collField.Type,
                                AdditionalFieldOfCollectionId = collField.Id
                            }).AsNoTracking().ToArray()
                }).AsNoTracking().SingleOrDefault();
            return item;
        }

        public async Task<bool> Create(ItemViewModel item)
        {
            var itemToAdd = new Item()
            {
                Title = item.Title,
                CollectionId = item.CollectionId
            };
            await _context.Items.AddAsync(itemToAdd);
            var res = _context.SaveChanges();
            if (res <= 0)
            {
                return false;
            }
            var fields = new List<FieldOfItem>();
            foreach (var field in item.Fields)
            {
                fields.Add(new FieldOfItem()
                {
                    ItemId = itemToAdd.Id,
                    Value = field.Value,
                    AdditionalFieldOfCollectionId = field.AdditionalFieldOfCollectionId
                });
            }
            await _context.FieldsOfItems.AddRangeAsync(fields);
            res += await _context.SaveChangesAsync();
            return res > 0;
        }

        public bool Edit(ItemViewModel item)
        {
            throw new NotImplementedException();
        }
    }
}
