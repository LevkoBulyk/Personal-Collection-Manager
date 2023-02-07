namespace Personal_Collection_Manager.Models
{
    public class ItemViewModel
    {
        public ItemViewModel()
        {
            Tags = new List<string>();
            StringFields = new List<string>();
            TextFields = new List<string>();
            NumberFields = new List<decimal>();
            DateTimeFields = new List<DateTime>();
            BoolFields = new List<bool>();
        }

        public int? Id { get; set; }
        public string Name { get; set; }
        public List<string> Tags { get; set; }
        public List<string> StringFields { get; set; }
        public List<string> TextFields { get; set; }
        public List<decimal> NumberFields { get; set; }
        public List<DateTime> DateTimeFields { get; set; }
        public List<bool> BoolFields { get; set; }
    }
}
