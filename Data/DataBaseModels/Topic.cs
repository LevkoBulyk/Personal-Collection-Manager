using System.ComponentModel.DataAnnotations;

namespace Personal_Collection_Manager.Data.DataBaseModels
{
    public class Topic
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
    }
}
