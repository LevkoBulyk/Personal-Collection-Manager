using Personal_Collection_Manager.Models;
using System.Security.Claims;

namespace Personal_Collection_Manager.IRepository
{
    public interface ICollectionRepository
    {
        public Task<List<CollectionViewModel>> GetCollectionsOf(ClaimsPrincipal user);
        public Task<bool> Create(CollectionViewModel collection, ClaimsPrincipal collectionCreator);
        public bool Edit(CollectionViewModel collection);
        public CollectionViewModel GetCollectionById(int id);
        public CollectionViewModel GetCollectionByIdAsNoTraking(int id);
        public bool CollectionIsInDb(CollectionViewModel collection);
        public bool DeleteAdditionalField(int id);
        public bool Delete(int id);
        public List<string> GetTopicsWithPrefix(string prefix);
    }
}
