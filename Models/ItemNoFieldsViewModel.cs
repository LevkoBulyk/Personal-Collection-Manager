namespace Personal_Collection_Manager.Models
{
    public class ItemNoFieldsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string[] Tags { get; set; }
        public string AuthorId { get; set; }
        public string AuthorEmail { get; set; }
        public string CollectionTitle { get; set; }
        public int CollectionId { get; set; }
    }
}
