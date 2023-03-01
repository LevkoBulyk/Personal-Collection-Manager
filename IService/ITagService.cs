using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.IService
{
    public interface ITagService
    {
        public Task<List<TagViewModel>> GetTagsForCloud(int? itemId = null);
        public Task<List<Tag>> GetTagsWithPrefix(string prefix);
    }
}
