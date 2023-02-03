using System.ComponentModel.DataAnnotations;

namespace Personal_Collection_Manager.Models
{
    public class CollectionCreate
    {
        public CollectionCreate()
        {
            AdditionalFields = new AditionalField[0];
        }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Topic { get; set; }

        public string ImageUrl { get; set; }

        public string AuthorId { get; set; }

        public AditionalField[] AdditionalFields { get; set; }
    }
}
