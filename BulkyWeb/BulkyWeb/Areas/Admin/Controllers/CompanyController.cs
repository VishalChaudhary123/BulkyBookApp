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
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
      

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Index()
        {
            List<Company> objCompanysList = _unitOfWork.Company.GetAll().ToList();

            
            return View(objCompanysList );
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
           
            if(id == null || id == 0)
            {
                // Create
                return View(new Company());

            }
            else
            {
                // Update - populating the model based on passed id
                Company companyobj = _unitOfWork.Company.Get(u => u.Id == id);
                return View(companyobj);
            }
           
        }
        [HttpPost]
        [Route("[action]")]
        public IActionResult Upsert(Company obj) 
        {
            // Custom error message for name property.

           
       
            if (ModelState.IsValid)
            {
                

                if(obj.Id == 0)
                {
                    // Create

                    _unitOfWork.Company.Add(obj);
                }
                else
                {
                    // Update

                    _unitOfWork.Company.Update(obj);
                }
               
                _unitOfWork.Save();
                TempData["success"] = "Company created successfully.";
                return RedirectToAction("Index", "Company");
            }
            else
            {
               
                return View(obj);
            }
            //return View(); // Return back to view result to display errors

        }
        

       

        #region API CALLS
        
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAll()
        {
            List<Company> objCompanysList = _unitOfWork.Company.GetAll().ToList();
            return Json (new {Data = objCompanysList});
        }


        [HttpDelete]
        [Route("Delete")]
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            if(CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            
            _unitOfWork.Company.Delete(CompanyToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete successful" });

        }

        #endregion
    }
}
