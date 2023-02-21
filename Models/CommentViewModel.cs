namespace Personal_Collection_Manager.Models
{
    public class CommentViewModel
    {
        public int? Id { get; set; }
        public int ItemId { get; set; }
        public string AuthorEmail { get; set; }
        public string Text { get; set; }
    }
}
