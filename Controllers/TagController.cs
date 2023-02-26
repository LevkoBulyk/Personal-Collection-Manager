using Microsoft.AspNetCore.Mvc;
using Personal_Collection_Manager.IRepository;

namespace Personal_Collection_Manager.Controllers
{
    public class TagController : Controller
    {
        private readonly ITagRepository _tagRepository;

        public TagController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        public async Task<IActionResult> List(string prefix)
        {
            var list = (await _tagRepository.GetTagsWithPrefix(prefix))
                .Select(tag => tag.Value)
                .Take(10).ToList();
            return Json(list);
        }
    }
}
