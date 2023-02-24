namespace Personal_Collection_Manager.Data.DataBaseModels
{
    public class Like
    {
        public int ItemId { get; set; }
        public string UserId { get; set; }
        public bool ThumbUp { get; set; }
    }
}
