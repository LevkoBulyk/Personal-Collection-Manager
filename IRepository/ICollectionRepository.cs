using Personal_Collection_Manager.Models;
using System.Security.Claims;

namespace Personal_Collection_Manager.IRepository
{
    public interface ICollectionRepository
    {
        public Task<List<CollectionView>> GetCollectionsOf(ClaimsPrincipal user);
        public Task<bool> CreateCollecion(CollectionView collection, ClaimsPrincipal collectionCreator);
        public bool EditCollecion(CollectionView collection);
        public CollectionView GetCollectionById(int Id);
        public CollectionView GetCollectionByIdAsNoTraking(int Id);
        public bool CollectionIsInDb(CollectionView collection);
        public bool DeleteAdditionalField(int id);
        public bool DeleteCollection(int id);
    }
}
