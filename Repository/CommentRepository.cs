using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Personal_Collection_Manager.Data;
using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CommentRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(int Result, int commentId)> AddComment(CommentViewModel comment)
        {
            var author = await _dbContext.Users.FirstOrDefaultAsync(user => user.Email.Equals(comment.AuthorEmail));
            if (author == null) throw new ArgumentNullException(nameof(author));
            var commentToSave = new Comment()
            {
                ItemId = comment.ItemId,
                UserId = author.Id,
                Text = comment.Text
            };
            _dbContext.Comments.Add(commentToSave);
            return (await _dbContext.SaveChangesAsync(), commentToSave.Id);
        }

        public async Task<int> DeleteComment(int id)
        {
            var commentToDelete = await _dbContext.Comments.FindAsync(id);
            if (commentToDelete != null)
            {
                _dbContext.Comments.Remove(commentToDelete);
            }
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<int> EditComment(CommentViewModel comment)
        {
            var commentToEdit = await _dbContext.Comments
                .Where(c => c.Id == comment.Id)
                .SingleOrDefaultAsync();
            if (commentToEdit == null) throw new ArgumentNullException(nameof(commentToEdit));
            commentToEdit.Text = comment.Text;
            _dbContext.Comments.Update(commentToEdit);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task<List<CommentViewModel>> GetAllItemComments(int itemId)
        {
            return await _dbContext.Comments
                .Where(c => c.ItemId == itemId)
                .OrderByDescending(c => c.Id)
                .Join(_dbContext.Users,
                    comment => comment.UserId,
                    user => user.Id,
                    (comment, user) => new CommentViewModel()
                    {
                        Id = comment.Id,
                        ItemId = itemId,
                        AuthorEmail = user.Email,
                        Text = comment.Text
                    })
                .Select(c => c)
                .AsNoTracking().ToListAsync();
        }
    }
}
