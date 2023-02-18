using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.IRepository
{
    public interface IFieldOfItemRepository
    {
        public Task<ItemField[]> GetFieldsOfItemAsNoTraking(int itemId);
        public Task<int> CreateFieldsForItem(int itemId, ItemField[] fields);
        public Task<int> UpdateFieldsForItem(int itemId, ItemField[] fields);
        public Task<FieldOfItem> GetFieldById(int id);
    }
}
