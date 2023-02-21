using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.IService;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public Task<int> AddComment(CommentViewModel comment)
        {
            return _commentRepository.AddComment(comment);
        }

        public Task<int> DeleteComment(int id)
        {
            return _commentRepository.DeleteComment(id);
        }

        public Task<int> EditComment(CommentViewModel comment)
        {
            return _commentRepository.EditComment(comment);
        }

        public Task<List<CommentViewModel>> GetAllItemComments(int itemId)
        {
            return _commentRepository.GetAllItemComments(itemId);
        }
    }
}
