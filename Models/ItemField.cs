using Personal_Collection_Manager.Data.DataBaseModels.Enum;
using System.ComponentModel;

namespace Personal_Collection_Manager.Models
{
    public class ItemField
    {
        public int Order { get; set; }
        public string Title { get; set; }
        public string Value { get; set; }
        public FieldType Type { get; set; }
    }
}
