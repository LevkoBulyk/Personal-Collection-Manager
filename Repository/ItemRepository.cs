using Microsoft.EntityFrameworkCore;
using Personal_Collection_Manager.Data;
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
                                    Type = collField.Type
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
                        .Join(_context.AdditionalFieldsOfCollections, field => field.AdditionalFieldOfCollectionId, collField => collField.Id, (field, collField) => new ItemField()
                        {
                            Order = collField.Order,
                            Title = collField.Title,
                            Value = field.Value,
                            Type = collField.Type
                        }).ToArray()
                })
                .AsNoTracking()
                .SingleOrDefault();
            return item;
        }
    }
}
