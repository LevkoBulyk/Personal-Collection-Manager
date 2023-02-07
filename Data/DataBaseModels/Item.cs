using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Personal_Collection_Manager.Data.DataBaseModels
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Collection))]
        public int CollectionId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public bool Deleted { get; set; } = false;
    }
}
