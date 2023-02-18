namespace Personal_Collection_Manager.Models
{
    public class ItemListViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Values { get; set; }
    }
}
