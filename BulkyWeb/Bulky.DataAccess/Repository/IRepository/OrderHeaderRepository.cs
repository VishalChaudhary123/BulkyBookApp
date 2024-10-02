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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext db;

        public OrderHeaderRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }



        public void Update(OrderHeader obj)
        {
            db.OrderHeaders.Update(obj);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = db.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntendid)
        {
            var orderFromDb = db.OrderHeaders.FirstOrDefault(x => x.Id == id);
            if (!string.IsNullOrEmpty(sessionId))
            {
                orderFromDb.SessionId = sessionId;

            }
            if (!string.IsNullOrEmpty(paymentIntendid))
            {
                {
                    orderFromDb.PaymentIntendId = paymentIntendid;
                    orderFromDb.PaymentDate = DateTime.Now;

                }
            }
        }
    }
}
