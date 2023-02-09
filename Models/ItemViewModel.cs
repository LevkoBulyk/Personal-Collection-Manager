using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Personal_Collection_Manager.Models
{
    public class ItemViewModel
    {
        public ItemViewModel()
        {
            Tags = new string[0];
            Fields = new ItemField[0];
        }

        public int? Id { get; set; }

        [Required]
        public int CollectionId { get; set; }

        [DisplayName("Title")]
        public string Title { get; set; }

        [DisplayName("Tags")]
        public string[] Tags { get; set; }

        [DisplayName("Fields")]
        public ItemField[] Fields { get; set; }
    }
}
