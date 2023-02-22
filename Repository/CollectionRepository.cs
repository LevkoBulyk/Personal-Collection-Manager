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
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        //private readonly ILogger _logger;

        public CollectionRepository(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager/*,
            ILogger logger*/)
        {
            _dbContext = context;
            _userManager = userManager;
            //_logger = logger;
        }

        public bool CollectionIsInDb(CollectionViewModel collection)
        {
            if (collection.Id == null)
            {
                return false;
            }
            return _dbContext.Collections.Find(collection.Id) != null;
        }

        public async Task<bool> Create(CollectionViewModel collection, ClaimsPrincipal collectionCreator)
        {
            var currentUserId = (await _userManager.GetUserAsync(collectionCreator)).Id;
            var topic = _dbContext.Topics.FirstOrDefault(t => t.Title.Equals(collection.Topic));
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
            _dbContext.Collections.Add(col);
            var res = _dbContext.SaveChanges();
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
            _dbContext.AdditionalFieldsOfCollections.AddRange(additionalFields);
            return (res + _dbContext.SaveChanges()) > 0;
        }

        public bool DeleteAdditionalField(int id)
        {
            var field = (from f in _dbContext.AdditionalFieldsOfCollections
                         where f.Id == id
                         select f).SingleOrDefault();
            if (field != null)
            {
                field.Deleted = true;
                _dbContext.AdditionalFieldsOfCollections.Update(field);
            }
            return _dbContext.SaveChanges() > 0;
        }

        public bool Delete(int id)
        {
            var collection = (from c in _dbContext.Collections
                              where c.Id == id
                              select c).SingleOrDefault();
            if (collection != null)
            {
                collection.Deleted = true;
                _dbContext.Collections.Update(collection);
                var fields = (from f in _dbContext.AdditionalFieldsOfCollections
                              where f.CollectionId == id
                              select f).ToList();
                foreach (var field in fields)
                {
                    field.Deleted = true;
                    _dbContext.AdditionalFieldsOfCollections.Update(field);
                }
            }
            return _dbContext.SaveChanges() > 0;
        }

        public bool Edit(CollectionViewModel input)
        {
            var topic = _dbContext.Topics.First(t => t.Title.Equals(input.Topic));
            if (topic == null)
            {
                throw new TopicNotFoundException($"Topic '{input.Topic}' not found in DB. You must choose topic from the drop-down list");
            }
            var collection = _dbContext.Collections.Find(input.Id);
            if (collection == null)
            {
                throw new ArgumentException(nameof(input.Id));
            }
            collection.Id = (int)input.Id;
            collection.Title = input.Title;
            collection.Description = input.Description;
            collection.ImageUrl = input.ImageUrl;
            collection.TopicId = topic.Id;
            _dbContext.Collections.Update(collection);
            foreach (var inputField in input.AdditionalFields)
            {
                var field = _dbContext.AdditionalFieldsOfCollections.Find(inputField.Id);
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
                    _dbContext.Add(createField);
                }
                else
                {
                    field.Title = inputField.Name;
                    field.Type = inputField.Type;
                    field.Order = inputField.Order;
                    _dbContext.Update(field);
                }
            }
            return _dbContext.SaveChanges() > 0;
        }

        public CollectionViewModel GetCollectionById(int id)
        {
            var collection = (from c in _dbContext.Collections
                              where c.Id == id && !c.Deleted
                              join t in _dbContext.Topics
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
                                  AdditionalFields = (from f in _dbContext.AdditionalFieldsOfCollections
                                                      where f.CollectionId == id && !f.Deleted
                                                      orderby f.Order
                                                      select new AditionalField()
                                                      {
                                                          // TODO: use mapper
                                                          Id = f.Id,
                                                          Name = f.Title,
                                                          Type = f.Type,
                                                          QuantityOfItems = _dbContext.FieldsOfItems
                                                            .Where(field => field.AdditionalFieldOfCollectionId == f.Id && !field.Deleted)
                                                            .Count(),
                                                          Order = f.Order
                                                      }).ToArray()
                              }).SingleOrDefault();
            return collection;
        }

        public CollectionViewModel GetCollectionByIdAsNoTraking(int id)
        {
            var collection = _dbContext.Collections
                .AsNoTracking()
                .Where(c => c.Id == id && !c.Deleted)
                .Join(_dbContext.Topics, c => c.TopicId, t => t.Id, (c, t) => new CollectionViewModel()
                {
                    // TODO: use mapper
                    Id = c.Id,
                    UserId = c.UserId,
                    UserName = (from user in _dbContext.Users
                                where user.Id.Equals(c.UserId)
                                select user)
                                .Single().UserName,
                    Title = c.Title,
                    Description = c.Description,
                    Topic = t.Title,
                    ImageUrl = c.ImageUrl,
                    AdditionalFields = _dbContext.AdditionalFieldsOfCollections
                        .AsNoTracking()
                        .Where(f => f.CollectionId == id && !f.Deleted)
                        .OrderBy(f => f.Order)
                        .Select(f => new AditionalField()
                        {
                            // TODO: use mapper
                            Id = f.Id,
                            Name = f.Title,
                            Type = f.Type,
                            QuantityOfItems = _dbContext.FieldsOfItems
                                .Where(field => field.AdditionalFieldOfCollectionId == f.Id && !field.Deleted)
                                .Count(),
                            Order = f.Order
                        }).ToArray()
                }).SingleOrDefault();
            return collection;
        }


        public async Task<List<CollectionViewModel>> GetCollectionsOf(ClaimsPrincipal user)
        {
            var userId = (await _userManager.GetUserAsync(user)).Id;
            return await GetCollectionsOf(userId);
        }

        public async Task<List<CollectionViewModel>> GetCollectionsOf(string userId)
        {
            var collections = (from c in _dbContext.Collections
                               where c.UserId.Equals(userId) && !c.Deleted
                               join t in _dbContext.Topics
                               on c.TopicId equals t.Id
                               select new CollectionViewModel()
                               {
                                   // TODO: use mapper
                                   Id = c.Id,
                                   UserId = c.UserId,
                                   UserName = (from user in _dbContext.Users
                                               where user.Id.Equals(c.UserId)
                                               select user)
                                               .Single().UserName,
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
                res = (from t in _dbContext.Topics
                       select t.Title).Take(10).ToList();
            }
            else
            {
                res = (from t in _dbContext.Topics
                       where t.Title.StartsWith(prefix)
                       select t.Title).Take(10).ToList();
            }
            return res;
        }

        public async Task<List<CollectionViewModel>> GetCollections(int pageNumber, int countPerPage)
        {
            return await _dbContext.Collections
                .OrderByDescending(c => c.Id)
                .Select(c => new CollectionViewModel()
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    UserName = _dbContext.Users
                        .Where(u => u.Id.Equals(c.UserId))
                        .Single().UserName,
                    Title = c.Title,
                    Description = c.Description,
                    Topic = _dbContext.Topics
                        .Where(t => t.Id == c.TopicId)
                        .Single().Title,
                    ImageUrl = c.ImageUrl
                })
                .Skip(countPerPage * (pageNumber - 1))
                .Take(countPerPage).ToListAsync();

        }
    }
}
