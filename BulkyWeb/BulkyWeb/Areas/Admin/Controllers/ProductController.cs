using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Route("[controller]")]
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Index()
        {
            List<Product> objProductsList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

            
            return View(objProductsList );
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Upsert(int? id) // Update and Insert From the same action method
        {
            // Category list
            IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
           // ViewBag.CategoryList = CategoryList;

            ProductViewModel _productViewModel = new ProductViewModel()
            {
                CategoryList = CategoryList,
                Product = new Product()
            };
            if(id == null || id == 0)
            {
                // Create
                return View(_productViewModel);

            }
            else
            {
                // Update - populating the model based on passed id
                _productViewModel.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(_productViewModel);
            }
           
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Upsert(ProductViewModel obj, IFormFile? file) 
        {
            // Custom error message for name property.

           
       
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images\product");
                    if (!string.IsNullOrEmpty(obj.Product.ImageUrl))
                    {
                        // Delete the old image
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\'));// Removing the forward slash which is there in image saved path

                        if (System.IO.File.Exists(oldImagePath)) 
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using(var fileStream = new FileStream(Path.Combine(productPath, fileName),FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    obj.Product.ImageUrl = @"\images\product\" + fileName; // Saving the Path to the cell
                }
                if(obj.Product.Id == 0)
                {
                    // Create

                    _unitOfWork.Product.Add(obj.Product);
                }
                else
                {
                    // Update

                    _unitOfWork.Product.Update(obj.Product);
                }
               
                _unitOfWork.Save();
                TempData["success"] = "Product created successfully.";
                return RedirectToAction("Index", "Product");
            }
            else
            {
                // When the model is not valid, it goes back to the same view and expects the CategoryList which cannot be empty, so passing it
                obj.CategoryList = _unitOfWork.Category.GetAll().Select(o => new SelectListItem
                {
                    Text = o.Name,
                    Value = o.Id.ToString()
                });
                return View(obj);
            }
            //return View(); // Return back to view result to display errors

        }
        

        //[HttpGet]
        //[Route("[action]")]
        //public IActionResult Delete(int? Id)
        //{
        //    if (Id == null || Id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? Product = _unitOfWork.Product.Get(c => c.Id == Id);
        //    if (Product == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(Product);
        //}
        //[HttpPost]
        //[Route("[action]"), ActionName("Delete")]
        //public IActionResult DeletePost(int? Id)
        //{
        //    Product? Product = _unitOfWork.Product.Get(x => x.Id == Id);
        //    if (Product == null)
        //    {
        //        return NotFound();

        //    }
        //    _unitOfWork.Product.Delete(Product);
        //    _unitOfWork.Save();
        //    TempData["success"] = "Product deleted successfully.";
        //    return RedirectToAction("Index", "Product");


        //}

        #region API CALLS
        
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAll()
        {
            List<Product> objProductsList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json (new {Data = objProductsList});
        }


        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if(productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            // Before deleting removing old image

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Delete(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });

        }

        #endregion
    }
}
