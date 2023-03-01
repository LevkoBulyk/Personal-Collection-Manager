using Microsoft.AspNetCore.Mvc;
using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.IService;

namespace Personal_Collection_Manager.Controllers
{
    public class TagController : Controller
    {
        private readonly ITagService _tagService;

        public TagController(ITagService tagService)
        {
            _tagService = tagService;
        }

        public async Task<IActionResult> GetTagsForCloud(int? itemId = null)
        {
            var words = await _tagService.GetTagsForCloud(itemId);
            return Json(words);
        }

        public async Task<IActionResult> List(string prefix)
        {
            var list = (await _tagService.GetTagsWithPrefix(prefix))
                .Select(tag => tag.Value)
                .Take(10).ToList();
            return Json(list);
        }
    }
}
