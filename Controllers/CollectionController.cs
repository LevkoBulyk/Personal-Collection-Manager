using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Personal_Collection_Manager.IService;
using Personal_Collection_Manager.Models;
using Personal_Collection_Manager.Repository.Exceptions;

namespace Personal_Collection_Manager.Controllers
{
    public class CollectionController : Controller
    {
        private const string _success = "success";
        private const string _error = "error";
        private readonly ICollectionService _collectionService;

        public CollectionController(ICollectionService collectionService)
        {
            _collectionService = collectionService;
        }

        public IActionResult All(string? userId, int pageNumber)
        {
            if (userId == null)
            {
                var collections = _collectionService.GetTheBiggestCollections(pageNumber);
                return View();
            }
            return RedirectToAction("Index", "Dashboard", new { userId = userId });
        }

        public IActionResult Details(int id)
        {
            var collection = _collectionService.GetCollectionByIdAsNoTraking(id);
            return View(collection);
        }

        [Authorize]
        public IActionResult Edit(int? id)
        {
            var collection = _collectionService.GetCollectionById(id);
            return View(collection);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Edit(CollectionViewModel collection)
        {
            ModelState.Remove(nameof(collection.ImageUrl));
            if (!ModelState.IsValid)
            {
                TempData[_error] = "Not all required fields were filled, or some were filled with errors";
                return View(collection);
            }
            try
            {
                if (collection.Id == null)
                {
                    if (await _collectionService.Create(collection, User))
                    {
                        TempData[_success] = "New collection was successfully created";
                        return RedirectToAction("Index", "Dashboard");
                    }
                }
                else
                {
                    if (_collectionService.Edit(collection))
                    {
                        TempData[_success] = "Collection was saved";
                        return View("Edit", collection);
                    }
                    TempData[_error] = "Collection was not saved! Check your input! If it won't help, please contact admin, cause we are having serious problems";
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            catch (TopicNotFoundException e)
            {
                TempData[_error] = e.Message;
                return View(collection);
            }
            TempData[_error] = "Failed to save collection to the database";
            return View(collection);
        }

        [Authorize]
        public IActionResult Delete(int id)
        {
            _collectionService.Delete(id);
            TempData[_success] = "Collection was successfully deleted";
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        [Authorize]
        public IActionResult RemoveField(CollectionViewModel collection, int number)
        {
            var result = _collectionService.RemoveField(ref collection, number);
            if (result.Succeded)
            {
                TempData[_success] = result.Message;
                ModelState.Clear();
            }
            else
                TempData[_error] = result.Message;
            return View("Edit", collection);
        }

        [HttpPost]
        [Authorize]
        public IActionResult AddField(CollectionViewModel collection)
        {
            _collectionService.AddField(ref collection);
            ModelState.Clear();
            return View("Edit", collection);
        }

        [HttpPost]
        [Authorize]
        public IActionResult MoveUp(CollectionViewModel collection, int number)
        {
            _collectionService.MoveUp(ref collection, number);
            ModelState.Clear();
            return View("Edit", collection);
        }

        [HttpPost]
        [Authorize]
        public IActionResult MoveDown(CollectionViewModel collection, int number)
        {
            _collectionService.MoveDown(ref collection, number);
            ModelState.Clear();
            return View("Edit", collection);
        }

        public JsonResult Topics(string prefix)
        {
            var topics = _collectionService.GetTopicsWithPrefix(prefix);
            return Json(topics);
        }
    }
}
