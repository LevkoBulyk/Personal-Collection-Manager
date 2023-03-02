using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.IRepository
{
    public interface IFieldOfItemRepository
    {
        public Task<Models.ItemField[]> GetFieldsOfItemAsNoTraking(int itemId);
        public Task<int> CreateFieldsForItem(int itemId, Models.ItemField[] fields);
        public Task<int> UpdateFieldsForItem(int itemId, Models.ItemField[] fields);
        public Task<Data.DataBaseModels.FieldOfItem> GetFieldById(int id);
    }
}
