using Personal_Collection_Manager.Data.DataBaseModels.Enum;
using System.ComponentModel;

namespace Personal_Collection_Manager.Models
{
    public class ItemField
    {
        public ItemField()
        {
            switch (this.Type)
            {
                case FieldType.Number:
                    break;
                case FieldType.String:
                    break;
                case FieldType.MultyLineString:
                    break;
                case FieldType.DateTime:
                    break;
                case FieldType.Bool:
                    break;
                default:
                    break;
            }
        }
        public int Order { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
        public FieldType Type { get; set; }
        public int AdditionalFieldOfCollectionId { get; set; }
    }
}
