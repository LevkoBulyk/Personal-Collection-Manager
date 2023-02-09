using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.IService
{
    public interface IItemService
    {
        public ItemViewModel GetItemById(int? id);
        public ItemViewModel GetItemByIdAsNoTracking(int? id);
    }
}
