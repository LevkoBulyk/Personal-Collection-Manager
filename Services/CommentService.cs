using Microsoft.AspNetCore.SignalR;
using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.Hubs;
using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.IService;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ICommentHubService _commentHub;

        public CommentService(
            ICommentRepository commentRepository,
            ICommentHubService commentHub)
        {
            _commentRepository = commentRepository;
            _commentHub = commentHub;
        }

        public async Task<int> AddComment(CommentViewModel comment)
        {
            var res = await _commentRepository.AddComment(comment);
            _commentHub.NotifyAboutNewComment(comment.ItemId);
            return res.Result;
        }

        public async Task<int> DeleteComment(int id)
        {
            var res = await _commentRepository.DeleteComment(id);
            if (res > 0)
            {
                _commentHub.NotifyAboutDeletingComment(id);
            }
            return res;
        }

        public async Task<int> EditComment(CommentViewModel comment)
        {
            var res = await _commentRepository.EditComment(comment);
            if (res > 0)
            {
                _commentHub.NotifyAboutEditingComment((int)comment.Id, comment.Text);
            }
            return res;
        }

        public Task<List<CommentViewModel>> GetAllItemComments(int itemId)
        {
            return _commentRepository.GetAllItemComments(itemId);
        }
    }
}
