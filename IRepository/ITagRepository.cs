using Personal_Collection_Manager.Data.DataBaseModels;

namespace Personal_Collection_Manager.IRepository
{
    public interface ITagRepository
    {
        public Task<int> RemoveTagsOfItem(int itemId);
        public Task<int> AddTagsToItem(int itemId, List<string> tagValues);
        public List<Tag> GetTagsOfItem(int itemId);
        public Task<List<Tag>> GetTagsOfItemAsNoTraking(int itemId);
        public Task<int> Create(Tag tag);
        public Task<int> Update(Tag tag);
        public Task<int> Delete(int id);
    }
}
