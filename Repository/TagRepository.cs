using Microsoft.EntityFrameworkCore;
using Personal_Collection_Manager.Data;
using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.Repository
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public TagRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> AddTagsToItem(int itemId, List<string> tagValues)
        {
            var tagsToAdd = new List<Tag>();
            foreach (var tagValue in tagValues)
            {
                if (!string.IsNullOrEmpty(tagValue))
                {
                    tagsToAdd.Add(new Tag() { Value = tagValue });
                }
            }
            _dbContext.Tags.AddRange(tagsToAdd);
            var res = await _dbContext.SaveChangesAsync();
            var itemToTags = new List<ItemsTag>();
            foreach (var tag in tagsToAdd)
            {
                itemToTags.Add(new ItemsTag()
                {
                    ItemId = itemId,
                    TagId = tag.Id
                });
            }
            _dbContext.ItemsTags.AddRange(itemToTags);
            res += await _dbContext.SaveChangesAsync();
            return res;
        }

        public async Task<int> Create(Tag tag)
        {
            _dbContext.Tags.Add(tag);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> Delete(int id)
        {
            var tag = await _dbContext.Tags.FindAsync(id);
            _dbContext.Tags.Remove(tag);
            return await _dbContext.SaveChangesAsync();
        }

        public List<Tag> GetTagsOfItem(int itemId)
        {
            return (from itemTags in _dbContext.ItemsTags
                    where itemTags.ItemId == itemId
                    join tag in _dbContext.Tags on itemTags.TagId equals tag.Id
                    select tag).ToList();
        }

        public async Task<List<Tag>> GetTagsOfItemAsNoTraking(int itemId)
        {
            return await (_dbContext.ItemsTags
                          .Where(itemTag => itemTag.ItemId == itemId)
                          .Join(_dbContext.Tags,
                              itemTag => itemTag.TagId,
                              tag => tag.Id,
                              (itemTag, tag) => tag)
                          .Select(tag => tag)).ToListAsync();
        }

        public Task<List<Tag>> GetTagsWithPrefix(string prefix)
        {
            return _dbContext.Tags
                .Where(tag => tag.Value.StartsWith(prefix))
                .GroupBy(tag => tag.Value)
                .Select(group => group.First())
                .ToListAsync();
        }


        public async Task<int> RemoveTagsOfItem(int itemId)
        {
            var tagsToRemove = GetTagsOfItem(itemId);
            var itemToTags = await (from itemToTag in _dbContext.ItemsTags
                                    where itemToTag.ItemId == itemId
                                    select itemToTag).ToListAsync();
            _dbContext.ItemsTags.RemoveRange(itemToTags);
            var res = await _dbContext.SaveChangesAsync();
            _dbContext.Tags.RemoveRange(tagsToRemove);
            res += await _dbContext.SaveChangesAsync();
            return res;
        }

        public async Task<int> Update(Tag tag)
        {
            var tagToEdit = await _dbContext.Tags.FindAsync(tag.Id);
            tagToEdit.Value = tag.Value;
            _dbContext.Tags.Update(tagToEdit);
            return await _dbContext.SaveChangesAsync();
        }

        public Task<List<TagViewModel>> GetAllTagsForCloud()
        {
            return _dbContext.Tags
                .Select(tag => new TagViewModel()
                {
                    Text = tag.Value,
                    Link = "Search/Index/" + tag.Value,
                    Uses = _dbContext.Tags
                        .Where(t => t.Value.Equals(tag.Value))
                        .Count()
                }).Distinct().ToListAsync();
        }

        public Task<List<TagViewModel>> GetTagsForCloud(int? itemId)
        {
            if (itemId == null)
            {
                return _dbContext.Tags
                    .Select(tag => new TagViewModel()
                    {
                        Text = tag.Value,
                        Link = "Search/Index/" + tag.Value,
                        Uses = _dbContext.Tags
                            .Where(t => t.Value.Equals(tag.Value))
                            .Count()
                    }).Distinct().ToListAsync();
            }
            else
            {
                return _dbContext.Tags
                    .Join(_dbContext.ItemsTags,
                        tag => tag.Id,
                        itemTag => itemTag.TagId,
                        (tag, itemTag) => new { Tag = tag, itemTag.ItemId })
                    .Where(x => x.ItemId == itemId)
                    .Select(tag => new TagViewModel()
                    {
                        Text = tag.Tag.Value,
                        Link = "Search/Index/" + tag.Tag.Value,
                        Uses = _dbContext.Tags
                            .Where(t => t.Value.Equals(tag.Tag.Value))
                            .Count()
                    }).Distinct().ToListAsync();
            }
        }
    }
}
