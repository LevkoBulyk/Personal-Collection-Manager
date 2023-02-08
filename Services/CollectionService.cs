using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.IService;
using Personal_Collection_Manager.Models;
using System.Security.Claims;

namespace Personal_Collection_Manager.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly ICollectionRepository _collectionRepository;

        public CollectionService(ICollectionRepository collection)
        {
            _collectionRepository = collection;
        }

        public CollectionViewModel GetCollectionViewModel(int? id)
        {
            var collection = id == null ?
                    new CollectionViewModel() :
                    _collectionRepository.GetCollectionById((int)id);
            return collection;
        }

        public (bool Succeded, string Message) RemoveField(ref CollectionViewModel collection, int number)
        {
            int? id = collection.AdditionalFields[number].Id;
            if (id != null && !_collectionRepository.DeleteAdditionalField((int)id))
            {
                return (
                    Succeded: false,
                    Message: "Failed to delete additional field. Probably connection to the database was lost"
                    );
            }
            var count = collection.AdditionalFields.Length;
            var fields = collection.AdditionalFields;
            collection.AdditionalFields = new AditionalField[count - 1];
            for (int i = 0, j = 0; i < count; i++, j++)
            {
                if (i != number)
                {
                    collection.AdditionalFields[j] = fields[i];
                }
                else
                {
                    j--;
                }
            }
            return (
                    Succeded: true,
                    Message: "Additional field was successfully deleted"
                    );
        }

        public (bool Succeded, string Message) AddField(ref CollectionViewModel collection)
        {
            var count = collection.AdditionalFields.Length;
            if (count > 0)
            {
                var fields = collection.AdditionalFields;
                collection.AdditionalFields = new AditionalField[count + 1];
                collection.AdditionalFields[count] = new AditionalField();
                for (int i = 0; i < count; i++)
                {
                    collection.AdditionalFields[i] = fields[i];
                }
            }
            else
            {
                collection.AdditionalFields = new AditionalField[1] { new AditionalField() };
            }
            return (
                Succeded: true,
                Message: "Field added"
                );
        }

        public bool MoveUp(ref CollectionViewModel collection, int number)
        {
            var field = collection.AdditionalFields[number];
            collection.AdditionalFields[number] = collection.AdditionalFields[number - 1];
            collection.AdditionalFields[number - 1] = field;
            return true;
        }

        public bool MoveDown(ref CollectionViewModel collection, int number)
        {
            var field = collection.AdditionalFields[number];
            collection.AdditionalFields[number] = collection.AdditionalFields[number + 1];
            collection.AdditionalFields[number + 1] = field;
            return true;
        }

        public CollectionViewModel GetCollectionByIdAsNoTraking(int id)
        {
            return _collectionRepository.GetCollectionByIdAsNoTraking(id);
        }

        public Task<bool> Create(CollectionViewModel collection, ClaimsPrincipal collectionCreator)
        {
            return _collectionRepository.Create(collection, collectionCreator);
        }

        public bool Edit(CollectionViewModel collection)
        {
            return _collectionRepository.Edit(collection);
        }

        public bool Delete(int id)
        {
            return _collectionRepository.Delete(id);
        }

        public List<string> GetTopicsWithPrefix(string prefix)
        {
            return _collectionRepository.GetTopicsWithPrefix(prefix);
        }
    }
}
