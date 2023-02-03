using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Personal_Collection_Manager.Data.DataBaseModels
{
    public class CollectionTag
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Collection))]
        public int CollectionId { get; set; }

        [ForeignKey(nameof(Tag))]
        public int TagId { get; set; }

        public bool Deleted { get; set; } = false;
    }
}
