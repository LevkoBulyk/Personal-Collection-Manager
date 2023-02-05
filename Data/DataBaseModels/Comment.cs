using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Personal_Collection_Manager.Data.DataBaseModels
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Item))]
        public int ItemId { get; set; }

        [ForeignKey(nameof(ApplicationUser))]
        public string UserId { get; set; }

        [Required]
        public string Text { get; set; }

        public bool Deleted { get; set; } = false;
    }
}
