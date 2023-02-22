﻿using Microsoft.EntityFrameworkCore;
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
            return new ItemViewModel()
            {
                CollectionId = collectionId,
                Fields = (_dbContext.AdditionalFieldsOfCollections
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

        public async Task<ItemViewModel> GetItemByIdAsNoTracking(int id)
        {
            var item = _dbContext.Items
                .Where(i => i.Id == id && !i.Deleted)
                .Select(i => new ItemViewModel()
                {
                    Id = i.Id,
                    CollectionId = i.CollectionId,
                    Title = i.Title
                }).AsNoTracking().SingleOrDefault();
            item.Fields = await _fieldOfItemRepository.GetFieldsOfItemAsNoTraking(id);
            item.Tags = (from tag in (await _tagRepository.GetTagsOfItem(id))
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

        public async Task<List<ItemListViewModel>> GetAllItemsOfCollection(int collectionId)
        {
            return await _dbContext.Items
                .Where(item => item.CollectionId == collectionId && !item.Deleted)
                .OrderByDescending(item => item.Id)
                .Select(item => new ItemListViewModel()
                {
                    Id = item.Id,
                    Title = item.Title,
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
                }).AsNoTracking().ToListAsync();
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

        public async Task<List<ItemListViewModel>> GetItemsOfCollection(int collectionId, int page)
        {
            return await _dbContext.Items
                .Where(item => item.CollectionId == collectionId && !item.Deleted)
                .OrderByDescending(item => item.Id)
                .Select(item => new ItemListViewModel()
                {
                    Id = item.Id,
                    Title = item.Title,
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
                }).Skip(pageSize * (page - 1)).Take(pageSize)
                .AsNoTracking().ToListAsync();
        }
    }
}
