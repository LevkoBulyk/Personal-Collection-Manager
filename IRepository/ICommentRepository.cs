using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.IRepository
{
    public interface ICommentRepository
    {
        public Task<List<CommentViewModel>> GetAllItemComments(int itemId);
        public Task<(int Result, int commentId)> AddComment(CommentViewModel comment);
        public Task<int> DeleteComment(int id);
        public Task<int> EditComment(CommentViewModel comment);
    }
}
