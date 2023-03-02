using Microsoft.AspNetCore.SignalR;
using Personal_Collection_Manager.Hubs;
using Personal_Collection_Manager.IService;

namespace Personal_Collection_Manager.Services
{
    public class CommentHubService : ICommentHubService
    {
        private readonly IHubContext<CommentHub> _commentHub;

        public CommentHubService(IHubContext<CommentHub> commentHub)
        {
            _commentHub = commentHub;
        }

        public async Task NotifyAboutDeletingComment(int commentId)
        {
            await _commentHub.Clients.All.SendAsync("DeleteComment", commentId);
        }

        public async Task NotifyAboutEditingComment(int commentId, string newText)
        {
            await _commentHub.Clients.All.SendAsync("EditComment", commentId, newText);
        }

        public async Task NotifyAboutNewComment(int itemId)
        {
            await _commentHub.Clients.All.SendAsync("NewComment_" + itemId);
        }
    }
}
