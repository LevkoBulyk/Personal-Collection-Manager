using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.IService;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.Controllers
{
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CommentController(
            ICommentService commentService,
            UserManager<ApplicationUser> userManager)
        {
            _commentService = commentService;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<bool> Add(int itemId, string text)
        {
            var email = _userManager.GetUserAsync(HttpContext.User).Result.Email;
            var comment = new CommentViewModel()
            {
                AuthorEmail = email,
                Text = text,
                ItemId = itemId
            };
            await _commentService.AddComment(comment);
            return true;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCommentsForItem(int itemId)
        {
            var comments = await _commentService.GetAllItemComments(itemId);
            return PartialView("_CommentsPartial", comments);
        }

        [HttpGet]
        public async Task<int> EditComment(int commentId, string text)
        {
            var comment = new CommentViewModel()
            {
                Id = commentId,
                Text = text
            };
            return await _commentService.EditComment(comment);
        }

        [HttpGet]
        public async Task<int> DeleteComment(int commentId)
        {
            return await _commentService.DeleteComment(commentId);
        }
    }
}
