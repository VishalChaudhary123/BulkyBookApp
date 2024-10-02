using Bulky.DataAccess.Repository.IRepository;
using Bulky.Utility;
using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Route("[controller]")]
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Index()
        {
            List<Category> objCategoriesList = _unitOfWork.Category.GetAll().ToList();
            return View(objCategoriesList);
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Create(Category obj)
        {
            // Custom error message for name property.

            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "DisplayOrder cannot exactly match the name.");
            }
            if (obj.Name != null && obj.Name.ToLower() == "test")
            {
                ModelState.AddModelError("name", "Test is an invalid value.");
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully.";
                return RedirectToAction("Index", "Category");
            }
            return View(); // Return back to view result to display errors

        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Edit(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            Category? category = _unitOfWork.Category.Get(c => c.Id == Id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Edit(Category obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category updated successfully.";
                return RedirectToAction("Index", "Category");
            }
            return View();
        }

        [HttpGet]
        [Route("[action]")]
        public IActionResult Delete(int? Id)
        {
            if (Id == null || Id == 0)
            {
                return NotFound();
            }
            Category? category = _unitOfWork.Category.Get(c => c.Id == Id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
        [HttpPost]
        [Route("[action]"), ActionName("Delete")]
        public IActionResult DeletePost(int? Id)
        {
            Category? category = _unitOfWork.Category.Get(x => x.Id == Id);
            if (category == null)
            {
                return NotFound();

            }
            _unitOfWork.Category.Delete(category);
            _unitOfWork.Save();
            TempData["success"] = "Category deleted successfully.";
            return RedirectToAction("Index", "Category");


        }
    }
}
