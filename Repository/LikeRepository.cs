using Microsoft.EntityFrameworkCore;
using Personal_Collection_Manager.Data;
using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.IRepository;

namespace Personal_Collection_Manager.Repository
{
    public class LikeRepository : ILikeRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public LikeRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<int> CountLikesOfItemAsync(int itemId)
        {
            return _dbContext.Likes
                .Where(like => like.ItemId == itemId && like.ThumbUp)
                .CountAsync();
        }

        public Task<int> ThumbDown(int itemId, string userId)
        {
            return GiveLike(itemId, userId, false);
        }

        public Task<int> ThumbUp(int itemId, string userId)
        {
            return GiveLike(itemId, userId, false);
        }

        private async Task<int> GiveLike(int itemId, string userId, bool thumbUp)
        {
            var like = await FindLike(itemId, userId);
            if (like == null)
            {
                like = new Like()
                {
                    UserId = userId,
                    ItemId = itemId,
                    ThumbUp = thumbUp
                };
                _dbContext.Likes.Add(like);
            }
            else
            {
                if (like.ThumbUp != thumbUp)
                {
                    like.ThumbUp = thumbUp;
                    _dbContext.Likes.Update(like);
                }
                else
                {
                    _dbContext.Likes.Remove(like);
                }
            }
            return await _dbContext.SaveChangesAsync();
        }

        private Task<Like?> FindLike(int itemId, string userId)
        {
            return _dbContext.Likes
                .Where(like => like.UserId.Equals(userId) && like.ItemId == itemId)
                .SingleOrDefaultAsync();
        }

    }
}
