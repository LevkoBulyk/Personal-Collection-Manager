using System.ComponentModel.DataAnnotations;

namespace Personal_Collection_Manager.Models
{
    public class CollectionView
    {
        public CollectionView()
        {
            AdditionalFields = new AditionalField[0];
        }

        public int? Id { get; set; }

        public string? UserId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Topic { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public AditionalField[] AdditionalFields { get; set; }

        public string? ReturnUrl { get; set; }

    }
}
