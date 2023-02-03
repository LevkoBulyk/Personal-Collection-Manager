using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Personal_Collection_Manager.Data.DataBaseModels;
using System.Reflection.Emit;

namespace Personal_Collection_Manager.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Tag> Tags { get; set; }
        public DbSet<Field> AditionalFields { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CollectionTag> CollectionsTags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Collection>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(collection => collection.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Item>()
                .HasOne<Collection>()
                .WithMany()
                .HasForeignKey(item => item.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Field>()
                .HasOne<Item>()
                .WithMany()
                .HasForeignKey(field => field.FieldId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne<Item>()
                .WithMany()
                .HasForeignKey(comment => comment.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(comment => comment.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<CollectionTag>()
                .HasOne<Collection>()
                .WithMany()
                .HasForeignKey(ct => ct.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CollectionTag>()
                .HasOne<Tag>()
                .WithMany()
                .HasForeignKey(ct => ct.TagId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}