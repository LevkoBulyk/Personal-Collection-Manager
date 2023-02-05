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

        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<FieldOfItem> FieldsOfItems { get; set; }
        public DbSet<AdditionalFieldOfCollection> AdditionalFieldsOfCollections { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ItemsTag> ItemsTags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Collection>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(collection => collection.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Item>()
                .HasOne<Collection>()
                .WithMany()
                .HasForeignKey(item => item.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FieldOfItem>()
                .HasOne<Item>()
                .WithMany()
                .HasForeignKey(field => field.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne<Item>()
                .WithMany()
                .HasForeignKey(comment => comment.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(comment => comment.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ItemsTag>()
                .HasOne<Item>()
                .WithMany()
                .HasForeignKey(ct => ct.ItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ItemsTag>()
                .HasOne<Tag>()
                .WithMany()
                .HasForeignKey(ct => ct.TagId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<AdditionalFieldOfCollection>()
                .HasOne<Collection>()
                .WithMany()
                .HasForeignKey(additionalField => additionalField.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<FieldOfItem>()
                .HasOne<AdditionalFieldOfCollection>()
                .WithMany()
                .HasForeignKey(f => f.AdditionalFieldOfCollectionId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}