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
        private readonly ICollectionService _collection;

        public CollectionController(ICollectionService collection)
        {
            _collection = collection;
        }

        public IActionResult Detail(int id)
        {
            var collection = _collection.GetCollectionByIdAsNoTraking(id);
            return View(collection);
        }

        public IActionResult Edit(int? id)
        {
            var collection = _collection.GetCollectionById(id);
            return View(collection);
        }

        [HttpPost]
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
                    if (await _collection.Create(collection, User))
                    {
                        TempData[_success] = "New collection was successfully created";
                        return RedirectToAction("Index", "Dashboard");
                    }
                }
                else
                {
                    if (_collection.Edit(collection))
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

        public IActionResult Delete(int id)
        {
            _collection.Delete(id);
            TempData[_success] = "Collection was successfully deleted";
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        public IActionResult RemoveField(CollectionViewModel collection, int number)
        {
            var result = _collection.RemoveField(ref collection, number);
            if (result.Succeded)
                TempData[_success] = result.Message;
            else
                TempData[_error] = result.Message;
            return View("Edit", collection);
        }

        [HttpPost]
        public IActionResult AddField(CollectionViewModel collection)
        {
            _collection.AddField(ref collection);
            return View("Edit", collection);
        }

        [HttpPost]
        public IActionResult MoveUp(CollectionViewModel collection, int number)
        {
            _collection.MoveUp(ref collection, number);
            ModelState.Clear();
            return View("Edit", collection);
        }

        [HttpPost]
        public IActionResult MoveDown(CollectionViewModel collection, int number)
        {
            _collection.MoveDown(ref collection, number);
            ModelState.Clear();
            return View("Edit", collection);
        }

        public JsonResult Topics(string prefix)
        {
            var topics = _collection.GetTopicsWithPrefix(prefix);
            return Json(topics);
        }
    }
}
