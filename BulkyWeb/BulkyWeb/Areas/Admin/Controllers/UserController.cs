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
using Microsoft.VisualBasic;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Route("[controller]")]
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext db;

        public UserController(IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            this.db = db;
        }
        [HttpGet]
        [Route("[action]")]
        public IActionResult Index()
        {
            return View();
        }





        #region API CALLS

        [HttpGet]
        [Route("[action]")]
        public IActionResult GetAll()
        {
            List<ApplicationUser> objUserList = _unitOfWork.ApplicationUser.GetAll(includeProperties: "Company").ToList();

            var userRoles = db.UserRoles.ToList(); // UserRole table i.e aspnetUserRoles
            var roles = db.Roles.ToList(); // AspNetRoles table

            foreach (var user in objUserList)
            {
                var roleId = userRoles.FirstOrDefault(x => x.UserId == user.Id).RoleId;
               user.Role= roles.FirstOrDefault(x => x.Id == roleId).Name;
                // Check if the Company property is null
                if (user.Company == null)
                {
                    user.Company = new Company { Name = "" };
                }
                else if (user.Company.Name == null)
                {
                    user.Company.Name = ""; // Initialize Name if it's null
                }
            }
            return Json(new { Data = objUserList });
        }



        [HttpPost]
        [Route("LockUnlock")]
        public IActionResult LockUnlock([FromBody] string userid)
        {
            var objFromDb = _unitOfWork.ApplicationUser.Get(u => u.Id == userid);
            if(objFromDb == null)
            {
                return Json(new { success = false, message = "Error while locking/unloacking" });
            }
            if(objFromDb.LockoutEnd !=null && objFromDb.LockoutEnd > DateTime.Now)
            {
                //user is currently locked and we need to unlock them
                objFromDb.LockoutEnd = DateTime.Now;
            }
            else
            {
                objFromDb.LockoutEnd = DateTime.Now.AddYears(100);
            }
            _unitOfWork.Save();
            return Json(new { success = true, message = "Delete successful" });

        }

        #endregion
    }
}
