using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Personal_Collection_Manager.Data.DataBaseModels
{
    public class FieldOfItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Item))]
        public int ItemId { get; set; }

        [ForeignKey(nameof(AdditionalFieldOfCollection))]
        public int AdditionalFieldOfCollectionId { get; set; }

        [Required]
        public string Value { get; set; }

        public bool Deleted { get; set; } = false;
    }
}
