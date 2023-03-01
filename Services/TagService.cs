using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.IService;
using Personal_Collection_Manager.Models;
using System.Drawing;

namespace Personal_Collection_Manager.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;

        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<List<TagViewModel>> GetTagsForCloud(int? itemId)
        {
            double fontMax = 35;
            double fontMin = 15;
            var tags = await _tagRepository.GetTagsForCloud(itemId);
            int maxUses = tags.Max(t => t.Uses);
            int minUses = tags.Min(t => t.Uses);
            foreach (var tag in tags)
            {
                tag.Weight = tag.Uses == minUses ? fontMin :
                    (fontMax - fontMin) * tag.Uses / maxUses + fontMax;
            }
            return tags;
        }

        public Task<List<Tag>> GetTagsWithPrefix(string prefix)
        {
            return _tagRepository.GetTagsWithPrefix(prefix);
        }
    }
}
