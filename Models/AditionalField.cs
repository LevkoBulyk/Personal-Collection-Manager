using Personal_Collection_Manager.Data.DataBaseModels.Enum;
using System.ComponentModel.DataAnnotations;

namespace Personal_Collection_Manager.Models
{
    public class AditionalField
    {
        public int? Id { get; set; }
        
        [Required]
        public FieldType Type { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int Order { get; set; }
    }
}
