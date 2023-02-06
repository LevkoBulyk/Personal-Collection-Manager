using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Personal_Collection_Manager.Data;
using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.Data.DataBaseModels.Enum;
using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.IService;
using Personal_Collection_Manager.Models;
using System.Security.Claims;

namespace Personal_Collection_Manager.Repository
{
    public class CollectionRepository : ICollectionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IPhotoService _photoService;
        //private readonly ILogger _logger;

        public CollectionRepository(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IPhotoService photoService/*,
            ILogger logger*/)
        {
            _context = context;
            _userManager = userManager;
            _photoService = photoService;
            //_logger = logger;
        }

        public bool CollectionIsInDb(CollectionViewModel collection)
        {
            if (collection.Id == null)
            {
                return false;
            }
            return _context.Collections.Find(collection.Id) != null;
        }

        public async Task<bool> Create(CollectionViewModel collection, ClaimsPrincipal collectionCreator)
        {
            var currentUserId = (await _userManager.GetUserAsync(collectionCreator)).Id;
            var photoResult = collection.Image == null ? null : await _photoService.AddPhotoAsync(collection.Image);
            var col = new Collection()
            {
                UserId = currentUserId,
                Name = collection.Name,
                Description = collection.Description,
                Topic = collection.Topic,
                ImageUrl = photoResult == null ? string.Empty : photoResult.Url.ToString()
            };
            _context.Collections.Add(col);
            var res = _context.SaveChanges();
            if (res <= 0)
            {
                return false;
            }
            var additionalFields = new List<AdditionalFieldOfCollection>();
            foreach (var field in collection.AdditionalFields)
            {
                additionalFields.Add(new AdditionalFieldOfCollection()
                {
                    CollectionId = col.Id,
                    Name = field.Name,
                    Type = field.Type,
                    Order = field.Order
                });
            }
            _context.AdditionalFieldsOfCollections.AddRange(additionalFields);
            return (res + _context.SaveChanges()) > 0;
        }

        public bool DeleteAdditionalField(int id)
        {
            var field = (from f in _context.AdditionalFieldsOfCollections
                         where f.Id == id
                         select f).SingleOrDefault();
            if (field != null)
                _context.AdditionalFieldsOfCollections.Remove(field);
            return _context.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var collection = (from c in _context.Collections
                              where c.Id == id
                              select c).SingleOrDefault();
            if (collection != null)
            {
                _context.Collections.Remove(collection);
                var res = _photoService.DeletePhoto(collection.ImageUrl);
                Console.WriteLine($"Cloude delete: result: {res.Result}");
                var fields = (from f in _context.AdditionalFieldsOfCollections
                              where f.CollectionId == id
                              select f).ToList();
                foreach (var field in fields)
                {
                    _context.AdditionalFieldsOfCollections.Remove(field);
                }
            }
            return _context.SaveChanges() > 0;
        }

        public bool Edit(CollectionViewModel input)
        {
            if (input.Id == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            var collection = _context.Collections.Find(input.Id);
            if (collection == null)
            {
                throw new ArgumentException(nameof(input.Id));
            }
            if (collection.ImageUrl.Length > 0)
            {
                _photoService.DeletePhoto(collection.ImageUrl);
            }
            if (input.Image != null)
            {
                var uploadResult = _photoService.AddPhoto(input.Image);
                collection.Id = (int)input.Id;
                collection.ImageUrl = uploadResult.Url.ToString();
            }
            else
            {
                collection.ImageUrl = string.Empty;
            }
            collection.Name = input.Name;
            collection.Description = input.Description;
            collection.Topic = input.Topic;
            _context.Collections.Update(collection);
            foreach (var inputField in input.AdditionalFields)
            {
                var field = _context.AdditionalFieldsOfCollections.Find(inputField.Id);
                if (field == null)
                {
                    var createField = new AdditionalFieldOfCollection()
                    {
                        CollectionId = collection.Id,
                        Name = inputField.Name,
                        Type = inputField.Type,
                        Order = inputField.Order
                    };
                    _context.Add(createField);
                }
                else
                {
                    field.Name = inputField.Name;
                    field.Type = inputField.Type;
                    field.Order = inputField.Order;
                    _context.Update(field);
                }
            }
            return _context.SaveChanges() > 0;
        }

        public CollectionViewModel GetCollectionById(int Id)
        {
            var collection = (from c in _context.Collections
                              where c.Id == Id
                              select new CollectionViewModel()
                              {
                                  Id = c.Id,
                                  UserId = c.UserId,
                                  Name = c.Name,
                                  Description = c.Description,
                                  Topic = c.Topic,
                                  ImageUrl = c.ImageUrl,
                              }).SingleOrDefault();
            var additionalFields = (from f in _context.AdditionalFieldsOfCollections
                                    where f.CollectionId == Id
                                    orderby f.Order
                                    select new AditionalField()
                                    {
                                        Id = f.Id,
                                        Name = f.Name,
                                        Type = f.Type,
                                        Order = f.Order
                                    }).ToArray();
            collection.AdditionalFields = additionalFields;
            return collection;
        }

        public CollectionViewModel GetCollectionByIdAsNoTraking(int Id)
        {
            var collection = _context.Collections
                .AsNoTracking()
                .Where(c => c.Id == Id)
                .Select(c => new CollectionViewModel()
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    Name = c.Name,
                    Description = c.Description,
                    Topic = c.Topic,
                    ImageUrl = c.ImageUrl,
                })
                .SingleOrDefault();
            var additionalFields = _context.AdditionalFieldsOfCollections
                .AsNoTracking()
                .Where(f => f.CollectionId == Id)
                .OrderBy(f => f.Order)
                .Select(f => new AditionalField()
                {
                    Id = f.Id,
                    Name = f.Name,
                    Type = f.Type,
                    Order = f.Order
                })
                .ToArray();
            collection.AdditionalFields = additionalFields;
            return collection;
        }

        public async Task<List<CollectionViewModel>> GetCollectionsOf(ClaimsPrincipal user)
        {
            var Id = (await _userManager.GetUserAsync(user)).Id;
            var collections = (from coll in _context.Collections
                               where coll.UserId.Equals(Id)
                               select new CollectionViewModel()
                               {
                                   Id = coll.Id,
                                   Name = coll.Name,
                                   Description = coll.Description,
                                   Topic = coll.Topic,
                                   ImageUrl = coll.ImageUrl
                               }).ToList();
            return collections;
        }
    }
}
