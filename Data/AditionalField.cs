using Microsoft.VisualBasic.FileIO;
using Personal_Collection_Manager.Data.DataBaseModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Personal_Collection_Manager.Data
{
    public class AdditionalFieldOfCollection
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Collection))]
        public int CollectionId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public FieldType Type { get; set; }

        public bool Deleted { get; set; } = false;
    }
}
