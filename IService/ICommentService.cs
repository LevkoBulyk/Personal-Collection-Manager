using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.IService
{
    public interface ICommentService
    {
        public Task<List<CommentViewModel>> GetAllItemComments(int itemId);
        public Task<int> AddComment(CommentViewModel comment);
        public Task<int> DeleteComment(int id);
        public Task<int> EditComment(CommentViewModel comment);
    }
}
