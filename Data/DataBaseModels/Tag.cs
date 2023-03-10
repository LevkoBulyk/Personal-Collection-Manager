using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Personal_Collection_Manager.Data.DataBaseModels
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Value { get; set; }
    }
}
