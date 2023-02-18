using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using Personal_Collection_Manager.Data;
using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.Models;
using System.Linq;

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
                            Fields = (from foi in _context.FieldsOfItems
                                      where foi.ItemId == id
                                      join afoc in _context.AdditionalFieldsOfCollections
                                      on foi.AdditionalFieldOfCollectionId equals afoc.Id
                                      where !afoc.Deleted
                                      select new ItemField
                                      {
                                          Order = afoc.Order,
                                          Title = afoc.Title,
                                          Value = foi.Value,
                                          Type = afoc.Type,
                                          AdditionalFieldOfCollectionId = afoc.Id
                                      }).ToArray()
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
                    Fields = (_context.AdditionalFieldsOfCollections
                .Where(afoc => afoc.CollectionId == i.CollectionId && !afoc.Deleted)
                .GroupJoin(_context.FieldsOfItems.Where(foi => foi.ItemId == i.Id),
                    afoc => afoc.Id,
                    foi => foi.AdditionalFieldOfCollectionId,
                    (afoc, fois) => new { AdditionalFieldOfCollection = afoc, FieldsOfItems = fois })
                .SelectMany(x => x.FieldsOfItems.DefaultIfEmpty(),
                    (afoc, foi) => new ItemField()
                    {
                        Order = afoc.AdditionalFieldOfCollection.Order,
                        Title = afoc.AdditionalFieldOfCollection.Title,
                        Value = foi != null ? foi.Value : "",
                        Type = afoc.AdditionalFieldOfCollection.Type,
                        AdditionalFieldOfCollectionId = foi != null ? foi.AdditionalFieldOfCollectionId : 0
                    })
                .OrderBy(x => x.Order)).ToArray()
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
            var tags = new List<Tag>();
            foreach (var tag in item.Tags)
            {
                if (!string.IsNullOrEmpty(tag))
                {
                    tags.Add(new Tag()
                    {
                        Value = tag
                    });
                }
            }
            await _context.Tags.AddRangeAsync(tags);
            res += await _context.SaveChangesAsync();
            var itemTags = new List<ItemsTag>();
            foreach (var tag in tags)
            {
                itemTags.Add(new ItemsTag()
                {
                    ItemId = itemToAdd.Id,
                    TagId = tag.Id
                });
            }
            await _context.ItemsTags.AddRangeAsync(itemTags);
            res += await _context.SaveChangesAsync();
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

        public async Task<bool> Edit(ItemViewModel item)
        {
            var itemToEdit = await _context.Items.FindAsync(item.Id);
            itemToEdit.Title = item.Title;
            _context.Items.Update(itemToEdit);
            var res = _context.SaveChanges();
            var tagsToRemove = (from tag in _context.Tags
                                join itemTag in _context.ItemsTags
                                on tag.Id equals itemTag.TagId
                                where itemTag.ItemId == item.Id
                                select tag).ToList();
            _context.Tags.RemoveRange(tagsToRemove);
            var itemTagsToRemove = (from itemTag in _context.ItemsTags
                                    where itemTag.ItemId == item.Id
                                    select itemTag).ToList();
            _context.ItemsTags.RemoveRange(itemTagsToRemove);
            await _context.SaveChangesAsync();

            var tags = new List<Tag>();
            foreach (var tag in item.Tags)
            {
                if (!string.IsNullOrEmpty(tag))
                {
                    tags.Add(new Tag()
                    {
                        Value = tag
                    });
                }
            }
            await _context.Tags.AddRangeAsync(tags);
            res += await _context.SaveChangesAsync();
            var itemTags = new List<ItemsTag>();
            foreach (var tag in tags)
            {
                itemTags.Add(new ItemsTag()
                {
                    ItemId = itemToEdit.Id,
                    TagId = tag.Id
                });
            }
            await _context.ItemsTags.AddRangeAsync(itemTags);
            res += await _context.SaveChangesAsync();

            var fieldsToUpdate = (from field in _context.FieldsOfItems
                                  where field.ItemId == item.Id
                                  join afoc in _context.AdditionalFieldsOfCollections
                                  on field.AdditionalFieldOfCollectionId equals afoc.Id
                                  where !afoc.Deleted
                                  orderby afoc.Order
                                  select field).ToArray();
            for (int i = 0; i < item.Fields.Length; i++)
            {
                fieldsToUpdate[i].Value = item.Fields[i].Value;
            }
            _context.FieldsOfItems.UpdateRange(fieldsToUpdate);

            res += await _context.SaveChangesAsync();
            return res > 0;
        }

        public async Task<List<ItemListViewModel>> GetGetItemsForCollection(int collectionId)
        {
            return await (_context.Items
                .Where(item => item.CollectionId == collectionId)
                .OrderByDescending(item => item.Id)
                .Select(item => new ItemListViewModel()
                {
                    Id = item.Id,
                    Title = item.Title,
                    Values = (_context.AdditionalFieldsOfCollections
                        .Where(afoc => afoc.CollectionId == collectionId && !afoc.Deleted)
                        .GroupJoin(_context.FieldsOfItems
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
                        .Select(x => x.Value)).ToList()
                }).AsNoTracking().ToListAsync());
        }
    }
}
