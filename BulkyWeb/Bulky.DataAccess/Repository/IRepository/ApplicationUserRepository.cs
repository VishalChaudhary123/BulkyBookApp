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
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext db;

        public ApplicationUserRepository(ApplicationDbContext db) : base(db) 
        {
            this.db = db;
        }
    }
}
