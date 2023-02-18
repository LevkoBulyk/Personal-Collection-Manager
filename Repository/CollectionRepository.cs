using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Personal_Collection_Manager.Data;
using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.Models;
using Personal_Collection_Manager.Repository.Exceptions;
using System.Security.Claims;

namespace Personal_Collection_Manager.Repository
{
    public class CollectionRepository : ICollectionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly ILogger _logger;

        public CollectionRepository(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager/*,
            ILogger logger*/)
        {
            _context = context;
            _userManager = userManager;
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
            var topic = _context.Topics.FirstOrDefault(t => t.Title.Equals(collection.Topic));
            if (topic == null)
            {
                throw new TopicNotFoundException($"Topic '{collection.Topic}' not found in DB. You must choose topic from the drop-down list");
            }
            var col = new Collection()
            {
                // TODO: use mapper
                UserId = currentUserId,
                Title = collection.Title,
                Description = collection.Description,
                TopicId = topic.Id,
                ImageUrl = collection.ImageUrl
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
                    // TODO: use mapper
                    CollectionId = col.Id,
                    Title = field.Name,
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
            {
                field.Deleted = true;
                _context.AdditionalFieldsOfCollections.Update(field);
            }
            return _context.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var collection = (from c in _context.Collections
                              where c.Id == id
                              select c).SingleOrDefault();
            if (collection != null)
            {
                collection.Deleted = true;
                _context.Collections.Update(collection);
                var fields = (from f in _context.AdditionalFieldsOfCollections
                              where f.CollectionId == id
                              select f).ToList();
                foreach (var field in fields)
                {
                    field.Deleted = true;
                    _context.AdditionalFieldsOfCollections.Update(field);
                }
            }
            return _context.SaveChanges() > 0;
        }

        public bool Edit(CollectionViewModel input)
        {
            var topic = _context.Topics.First(t => t.Title.Equals(input.Topic));
            if (topic == null)
            {
                throw new TopicNotFoundException($"Topic '{input.Topic}' not found in DB. You must choose topic from the drop-down list");
            }
            var collection = _context.Collections.Find(input.Id);
            if (collection == null)
            {
                throw new ArgumentException(nameof(input.Id));
            }
            collection.Id = (int)input.Id;
            collection.Title = input.Title;
            collection.Description = input.Description;
            collection.ImageUrl = input.ImageUrl;
            collection.TopicId = topic.Id;
            _context.Collections.Update(collection);
            foreach (var inputField in input.AdditionalFields)
            {
                var field = _context.AdditionalFieldsOfCollections.Find(inputField.Id);
                if (field == null)
                {
                    var createField = new AdditionalFieldOfCollection()
                    {
                        // TODO: use mapper
                        CollectionId = collection.Id,
                        Title = inputField.Name,
                        Type = inputField.Type,
                        Order = inputField.Order
                    };
                    _context.Add(createField);
                }
                else
                {
                    field.Title = inputField.Name;
                    field.Type = inputField.Type;
                    field.Order = inputField.Order;
                    _context.Update(field);
                }
            }
            return _context.SaveChanges() > 0;
        }

        public CollectionViewModel GetCollectionById(int id)
        {
            var collection = (from c in _context.Collections
                              where c.Id == id && !c.Deleted
                              join t in _context.Topics
                              on c.TopicId equals t.Id
                              select new CollectionViewModel()
                              {
                                  // TODO: use mapper
                                  Id = c.Id,
                                  UserId = c.UserId,
                                  Title = c.Title,
                                  Description = c.Description,
                                  Topic = t.Title,
                                  ImageUrl = c.ImageUrl,
                                  AdditionalFields = (from f in _context.AdditionalFieldsOfCollections
                                                      where f.CollectionId == id && !f.Deleted
                                                      orderby f.Order
                                                      select new AditionalField()
                                                      {
                                                          // TODO: use mapper
                                                          Id = f.Id,
                                                          Name = f.Title,
                                                          Type = f.Type,
                                                          Order = f.Order
                                                      }).ToArray()
                              }).SingleOrDefault();
            return collection;
        }

        public CollectionViewModel GetCollectionByIdAsNoTraking(int id)
        {
            var collection = _context.Collections
                .AsNoTracking()
                .Where(c => c.Id == id && !c.Deleted)
                .Join(_context.Topics, c => c.TopicId, t => t.Id, (c, t) => new CollectionViewModel()
                {
                    // TODO: use mapper
                    Id = c.Id,
                    UserId = c.UserId,
                    Title = c.Title,
                    Description = c.Description,
                    Topic = t.Title,
                    ImageUrl = c.ImageUrl,
                    AdditionalFields = _context.AdditionalFieldsOfCollections
                        .AsNoTracking()
                        .Where(f => f.CollectionId == id && !f.Deleted)
                        .OrderBy(f => f.Order)
                        .Select(f => new AditionalField()
                        {
                            // TODO: use mapper
                            Id = f.Id,
                            Name = f.Title,
                            Type = f.Type,
                            Order = f.Order
                        }).ToArray()
                }).SingleOrDefault();
            return collection;
        }

        public async Task<List<CollectionViewModel>> GetCollectionsOf(ClaimsPrincipal user)
        {
            var Id = (await _userManager.GetUserAsync(user)).Id;
            var collections = (from c in _context.Collections
                               where c.UserId.Equals(Id) && !c.Deleted
                               join t in _context.Topics
                               on c.TopicId equals t.Id
                               select new CollectionViewModel()
                               {
                                   // TODO: use mapper
                                   Id = c.Id,
                                   Title = c.Title,
                                   Description = c.Description,
                                   Topic = t.Title,
                                   ImageUrl = c.ImageUrl
                               }).ToList();
            return collections;
        }

        public List<string> GetTopicsWithPrefix(string prefix)
        {
            List<string> res;
            if (string.IsNullOrEmpty(prefix))
            {
                res = (from t in _context.Topics
                       select t.Title).Take(10).ToList();
            }
            else
            {
                res = (from t in _context.Topics
                       where t.Title.StartsWith(prefix)
                       select t.Title).Take(10).ToList();
            }
            return res;
        }
    }
}
