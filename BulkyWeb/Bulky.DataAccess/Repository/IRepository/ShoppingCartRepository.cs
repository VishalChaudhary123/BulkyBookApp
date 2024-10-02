using Bulky.Models.Models;
using BulkyWeb.Data;
using BulkyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext db;

        public ShoppingCartRepository (ApplicationDbContext db) : base(db) 
        {
            this.db = db;
        }

      

        public void Update(ShoppingCart obj)
        {
           db.ShoppingCarts.Update(obj);
        }
    }
}
