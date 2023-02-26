namespace Personal_Collection_Manager.IService
{
    public interface ICommentHubService
    {
        public Task NotifyAboutNewComment(int itemId);
        public Task NotifyAboutDeletingComment(int commentId);
        public Task NotifyAboutEditingComment(int commentId, string newText);
    }
}
