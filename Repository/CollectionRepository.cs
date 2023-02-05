﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Personal_Collection_Manager.Data;
using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.Data.DataBaseModels.Enum;
using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.Models;
using System.Security.Claims;

namespace Personal_Collection_Manager.Repository
{
    public class CollectionRepository : ICollectionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CollectionRepository(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public bool CollectionIsInDb(CollectionView collection)
        {
            if (collection.Id == null)
            {
                return false;
            }
            return _context.Collections.Find(collection.Id) != null;
        }

        public async Task<bool> CreateCollecion(CollectionView collection, ClaimsPrincipal collectionCreator)
        {
            var currentUserId = (await _userManager.GetUserAsync(collectionCreator)).Id;
            var col = new Collection()
            {
                UserId = currentUserId,
                Name = collection.Name,
                Description = collection.Description,
                Topic = collection.Topic,
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

        public bool DeleteCollection(int id)
        {
            var collection = (from c in _context.Collections
                             where c.Id == id
                             select c).SingleOrDefault();
            _context.Collections.Remove(collection);
            var fields = (from f in _context.AdditionalFieldsOfCollections
                         where f.CollectionId == id
                         select f).ToList();
            foreach (var field in fields)
            {
                _context.AdditionalFieldsOfCollections.Remove(field);
            }
            return _context.SaveChanges() > 0;
        }

        public bool EditCollecion(CollectionView input)
        {
            if (input.Id == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            var colllection = _context.Collections.Find(input.Id);
            if (colllection == null)
            {
                throw new ArgumentException(nameof(input.Id));
            }
            colllection.Id = (int)input.Id;
            colllection.Name = input.Name;
            colllection.Description = input.Description;
            colllection.Topic = input.Topic;
            colllection.ImageUrl = input.ImageUrl;
            _context.Collections.Update(colllection);
            foreach (var inputField in input.AdditionalFields)
            {
                var field = _context.AdditionalFieldsOfCollections.Find(inputField.Id);
                if (field == null)
                {
                    var createField = new AdditionalFieldOfCollection()
                    {
                        CollectionId = colllection.Id,
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

        public CollectionView GetCollectionById(int Id)
        {
            var collection = (from c in _context.Collections
                              where c.Id == Id
                              select new CollectionView()
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

        public CollectionView GetCollectionByIdAsNoTraking(int Id)
        {
            var collection = _context.Collections
                .AsNoTracking()
                .Where(c => c.Id == Id)
                .Select(c => new CollectionView()
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

        public async Task<List<CollectionView>> GetCollectionsOf(ClaimsPrincipal user)
        {
            var Id = (await _userManager.GetUserAsync(user)).Id;
            var collections = (from coll in _context.Collections
                               where coll.UserId.Equals(Id)
                               select new CollectionView()
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