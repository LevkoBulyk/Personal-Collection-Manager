using Personal_Collection_Manager.Data;
using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.Repository
{
    public class ItemRepository : IItemRepository
    {
        private readonly ApplicationDbContext _context;

        public ItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ItemViewModel GetItemById(int id)
        {
            var item = (from i in _context.Items
                        where i.Id == id && !i.Deleted
                        select i).SingleOrDefault();
            var result = new ItemViewModel() {
                Id = id,
                Name = item.Name
            };
            var additionalFields = (from af in _context.AdditionalFieldsOfCollections
                                    where af.CollectionId == item.CollectionId && !af.Deleted
                                    select af).ToList();

        }
    }
}
