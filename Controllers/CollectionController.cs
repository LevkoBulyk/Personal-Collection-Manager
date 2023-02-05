using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Personal_Collection_Manager.Data;
using Personal_Collection_Manager.Data.DataBaseModels;
using Personal_Collection_Manager.Data.DataBaseModels.Enum;
using Personal_Collection_Manager.IRepository;
using Personal_Collection_Manager.Models;

namespace Personal_Collection_Manager.Controllers
{
    public class CollectionController : Controller
    {
        private const string _success = "success";
        private const string _error = "error";
        private readonly ICollectionRepository _collection;

        public CollectionController(ICollectionRepository collection)
        {
            _collection = collection;
        }

        public IActionResult Edit(int? id)
        {
            var collection = id == null ?
                    new CollectionView() :
                    _collection.GetCollectionByIdAsNoTraking((int)id);
            collection.ReturnUrl = Request.Headers["Referer"].ToString();
            return View(collection);
        }

        public IActionResult Detail(int id)
        {
            var collection = _collection.GetCollectionByIdAsNoTraking(id);
            collection.ReturnUrl = Request.Headers["Referer"].ToString();
            return View(collection);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CollectionView collection)
        {
            if (!ModelState.IsValid)
            {
                TempData[_error] = "Not all required fields were filled, or some were filled with errors";
                return View(collection);
            }
            if (collection.Id == null)
            {
                if (await _collection.CreateCollecion(collection, User))
                {
                    TempData[_success] = "New collection was successfully created";
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            else
            {
                if (_collection.EditCollecion(collection))
                {
                    TempData[_success] = "Collection was edited and saved";
                    return RedirectToAction("Index", "Dashboard");
                }
            }
            TempData[_error] = "Failed to save collection to the database";
            return View(collection);
        }

        public IActionResult Delete(int id)
        {
            _collection.DeleteCollection(id);
            TempData[_success] = "Collection was successfully deleted";
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
        public IActionResult RemoveField(CollectionView collection, int number)
        {
            int? id = collection.AdditionalFields[number].Id;
            if (id != null && !_collection.DeleteAdditionalField((int)id))
            {
                TempData[_error] = "Failed to delete additional field. Probably connection to the database was lost";
                return View("Edit", collection);
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
            TempData[_success] = "Additional field was successfully deleted";
            return View("Edit", collection);
        }

        [HttpPost] 
        public IActionResult AddField(CollectionView collection)
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
            return View("Edit", collection);
        }

        [HttpPost]
        public IActionResult MoveUp(CollectionView collection, int number) 
        {
            var field = collection.AdditionalFields[number];
            collection.AdditionalFields[number] = collection.AdditionalFields[number - 1];
            collection.AdditionalFields[number - 1] = field;
            return View("Edit", collection);
        }

        [HttpPost]
        public IActionResult MoveDown(CollectionView collection, int number)
        {
            var field = collection.AdditionalFields[number];
            collection.AdditionalFields[number] = collection.AdditionalFields[number + 1];
            collection.AdditionalFields[number + 1] = field;
            return View("Edit", collection);
        }
    }
}
