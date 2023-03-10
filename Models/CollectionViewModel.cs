using System.ComponentModel.DataAnnotations;

namespace Personal_Collection_Manager.Models
{
    public class CollectionViewModel
    {
        public CollectionViewModel()
        {
            AdditionalFields = new AditionalField[0];
        }
        public int? Id { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public IFormFile? Image { get; set; } = null;
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string Topic { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int Likes { get; set; }
        public AditionalField[] AdditionalFields { get; set; }

    }
}
