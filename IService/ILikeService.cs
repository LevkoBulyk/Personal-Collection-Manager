namespace Personal_Collection_Manager.IService
{
    public interface ILikeService
    {
        public Task<int> CountLikesOfItemAsync(int itemId);
        public Task<int> ThumbUp(int itemId, string userId);
        public Task<int> ThumbDown(int itemId, string userId);
    }
}
