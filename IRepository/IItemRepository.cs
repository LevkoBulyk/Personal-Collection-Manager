using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.IRepository
{
    public interface IItemRepository
    {
        public ItemViewModel GetItemById(int id);
    }
}
