using BulkyWebRazor_Temp.Data;
using BulkyWebRazor_Temp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BulkyWebRazor_Temp.Pages.Categories
{
    [BindProperties]
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext db;
        public Category Category { get; set; }
        public DeleteModel(ApplicationDbContext db)
        {
            this.db = db;
        }
        public void OnGet(int? id)
        {
            if(id != null && id != 0)
            {

                db.Categories.Find(id);   
            }
           
        }
        public IActionResult OnPost() 
        {
            Category? obj = db.Categories.Find(Category.Id);
            if (obj == null) 
            {
              return NotFound(); 
            }
                db.Categories.Remove(Category);
                db.SaveChanges();
                return RedirectToPage("Index");
            
        }
    }
}
