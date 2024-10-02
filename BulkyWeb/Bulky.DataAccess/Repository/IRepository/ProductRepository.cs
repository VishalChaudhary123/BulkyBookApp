using Bulky.Models.Models;
using BulkyWeb.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext db;

        public ProductRepository(ApplicationDbContext db): base(db)
        {
            this.db = db;
        }

        public void Update(Product obj)
        {
            // db.Products.Update(obj);

            var objFromDb = db.Products.FirstOrDefault(x => x.Id == obj.Id);
            if (objFromDb != null) 
            {
               objFromDb.Title = obj.Title;
                objFromDb.Description = obj.Description;
                objFromDb.CategoryId = obj.CategoryId;
               objFromDb.ListPrice = obj.ListPrice;
                objFromDb.Price = obj.Price;
                objFromDb.Price100 = obj.Price100;
                objFromDb.Price50 = obj.Price50;
                objFromDb.Author = obj.Author;
                objFromDb.ISBN = obj.ISBN;
                if (obj.ImageUrl != null) 
                {
                   objFromDb.ImageUrl = obj.ImageUrl;
                } 
            }
        }

    }
}
