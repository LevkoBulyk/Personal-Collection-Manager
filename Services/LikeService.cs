using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.IService;

namespace Personal_Collection_Manager.Services
{
    public class LikeService : ILikeService
    {
        private readonly ILikeRepository _likeRepository;

        public LikeService(ILikeRepository likeRepository)
        {
            _likeRepository = likeRepository;
        }

        public Task<int> CountLikesOfItemAsync(int itemId)
        {
            return _likeRepository.CountLikesOfItemAsync(itemId);
        }

        public Task<int> ThumbDown(int itemId, string userId)
        {
            return _likeRepository.ThumbDown(itemId, userId);
        }

        public Task<int> ThumbUp(int itemId, string userId)
        {
            return _likeRepository.ThumbUp(itemId, userId);
        }
    }
}
