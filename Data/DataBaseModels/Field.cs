using Microsoft.VisualBasic.FileIO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Personal_Collection_Manager.Data.DataBaseModels
{
    public class Field
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Item))]
        public int FieldId { get; set; }

        [Required]
        public FieldType Type { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }

        [Required]
        public int Order { get; set; }

        public bool Deleted { get; set; } = false;
    }
}
