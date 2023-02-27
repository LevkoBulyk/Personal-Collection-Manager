using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.IService;
using Personal_Collection_Manager.Models;
using System.Security.Claims;

namespace Personal_Collection_Manager.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly ICollectionRepository _collectionRepository;
        private readonly IMarkdownService _markdownService;
        private readonly IPhotoService _photoService;

        public CollectionService(
            ICollectionRepository collectionRepository,
            IMarkdownService markdownService,
            IPhotoService photoService)
        {
            _collectionRepository = collectionRepository;
            _markdownService = markdownService;
            _photoService = photoService;
        }

        public CollectionViewModel GetCollectionById(int? id)
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
            var collection = _collectionRepository.GetCollectionByIdAsNoTraking(id);
            collection.Description = _markdownService.ToHtml(collection.Description);
            return collection;
        }

        public async Task<bool> Create(CollectionViewModel collection, ClaimsPrincipal collectionCreator)
        {
            var photoResult = collection.Image == null ? null : await _photoService.AddPhotoAsync(collection.Image);
            collection.ImageUrl = photoResult == null ? string.Empty : photoResult.Url.ToString();
            return await _collectionRepository.Create(collection, collectionCreator);
        }

        public bool Edit(CollectionViewModel collection)
        {
            if (collection.Id == null)
            {
                throw new ArgumentNullException(nameof(collection.Id));
            }
            var collectionToModify = GetCollectionByIdAsNoTraking((int)collection.Id);
            if (collectionToModify == null)
            {
                throw new ArgumentException(nameof(collection.Id));
            }
            collection.ImageUrl = collection.ImageUrl ?? "";
            if (collectionToModify.ImageUrl.Length > 0 && collection.ImageUrl.Length == 0)
            {
                _photoService.DeletePhoto(collectionToModify.ImageUrl);
            }
            if (collection.Image != null)
            {
                var uploadResult = _photoService.AddPhoto(collection.Image);
                collection.ImageUrl = uploadResult.Url.ToString();
            }
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

        public async Task<List<CollectionViewModel>> GetCollectionsOf(ClaimsPrincipal user)
        {
            var collections = await _collectionRepository.GetCollectionsOf(user);
            foreach (var collection in collections)
            {
                collection.Description = _markdownService.ToHtml(collection.Description);
            }
            return collections;
        }

        public async Task<List<CollectionViewModel>> GetCollectionsOf(string userId)
        {
            var collections = await _collectionRepository.GetCollectionsOf(userId);
            foreach (var collection in collections)
            {
                collection.Description = _markdownService.ToHtml(collection.Description);
            }
            return collections;
        }

        public Task<List<CollectionViewModel>> GetTheBiggestCollections(int pageNumber, int countPerPage)
        {
            return _collectionRepository.GetTheBiggestCollections(pageNumber, countPerPage);
        }
    }
}
