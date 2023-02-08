using Personal_Collection_Manager.Models;
using System.Security.Claims;

namespace Personal_Collection_Manager.IService
{
    public interface ICollectionService
    {
        public CollectionViewModel GetCollectionViewModel(int? id);
        public (bool Succeded, string Message) RemoveField(ref CollectionViewModel collection, int number);
        public (bool Succeded, string Message) AddField(ref CollectionViewModel collection);
        public bool MoveDown(ref CollectionViewModel collection, int number);
        public bool MoveUp(ref CollectionViewModel collection, int number);
        public CollectionViewModel GetCollectionByIdAsNoTraking(int id);
        public Task<bool> Create(CollectionViewModel collection, ClaimsPrincipal collectionCreator);
        public bool Edit(CollectionViewModel collection);
        public bool Delete(int id);
        public List<string> GetTopicsWithPrefix(string prefix);
    }
}
