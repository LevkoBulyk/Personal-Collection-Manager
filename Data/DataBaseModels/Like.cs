using System.ComponentModel.DataAnnotations;

namespace Personal_Collection_Manager.Data.DataBaseModels
{
    public class Like
    {
        [Key]
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string UserId { get; set; }
        public bool ThumbUp { get; set; }
    }
}
