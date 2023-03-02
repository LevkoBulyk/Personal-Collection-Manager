using Microsoft.EntityFrameworkCore;
using Personal_Collection_Manager.Data;
using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.Repository
{
    public class FieldOfItemRepository : IFieldOfItemRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public FieldOfItemRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CreateFieldsForItem(int itemId, Models.ItemField[] fields)
        {
            var fieldsToCreate = new List<Data.DataBaseModels.FieldOfItem>();
            foreach (var field in fields)
            {
                fieldsToCreate.Add(new Data.DataBaseModels.FieldOfItem()
                {
                    ItemId = itemId,
                    Value = field.Value,
                    AdditionalFieldOfCollectionId = field.AdditionalFieldOfCollectionId
                });
            }
            await _dbContext.FieldsOfItems.AddRangeAsync(fieldsToCreate);
            return (await _dbContext.SaveChangesAsync());
        }

        public Task<Data.DataBaseModels.FieldOfItem> GetFieldById(int id)
        {
            return _dbContext.FieldsOfItems.FirstAsync(field => field.Id == id);
        }

        public async Task<Models.ItemField[]> GetFieldsOfItemAsNoTraking(int itemId)
        {
            var collectionId = (await _dbContext.Items.FindAsync(itemId)).CollectionId;
            return await (_dbContext.AdditionalFieldsOfCollections
                .Where(afoc => afoc.CollectionId == collectionId && !afoc.Deleted)
                .GroupJoin(_dbContext.FieldsOfItems.Where(foi => foi.ItemId == itemId),
                    afoc => afoc.Id,
                    foi => foi.AdditionalFieldOfCollectionId,
                    (afoc, fois) => new { AdditionalFieldOfCollection = afoc, FieldsOfItems = fois })
                .SelectMany(x => x.FieldsOfItems.DefaultIfEmpty(),
                    (afoc, foi) => new ItemField()
                    {
                        Id = foi != null ? foi.Id : null,
                        Order = afoc.AdditionalFieldOfCollection.Order,
                        Title = afoc.AdditionalFieldOfCollection.Title,
                        Value = foi != null ? foi.Value : "",
                        Type = afoc.AdditionalFieldOfCollection.Type,
                        AdditionalFieldOfCollectionId = afoc.AdditionalFieldOfCollection.Id
                    })
                .OrderBy(x => x.Order)).ToArrayAsync();
        }

        public async Task<int> UpdateFieldsForItem(int itemId, Models.ItemField[] fields)
        {
            int res = 0;
            foreach (var field in fields)
            {
                if (field.Id != null)
                {
                    var fieldToUpdate = await GetFieldById((int)field.Id);
                    fieldToUpdate.Value = field.Value;
                    _dbContext.FieldsOfItems.Update(fieldToUpdate);
                    res += await _dbContext.SaveChangesAsync();
                }
                else
                {
                    var fieldToCreate = new Data.DataBaseModels.FieldOfItem()
                    {
                        ItemId = itemId,
                        AdditionalFieldOfCollectionId = field.AdditionalFieldOfCollectionId,
                        Value = field.Value
                    };
                    _dbContext.FieldsOfItems.Add(fieldToCreate);
                    res += await _dbContext.SaveChangesAsync();
                }
            }
            return res;
        }
    }
}
