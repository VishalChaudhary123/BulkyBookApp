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
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext db;

        public OrderDetailRepository(ApplicationDbContext db) : base(db) 
        {
            this.db = db;
        }

      

        public void Update(OrderDetail obj)
        {
           db.OrderDetails.Update(obj);
        }
    }
}
