using Bulky.Models.Models;
using BulkyWeb.Data;
using BulkyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext db;

        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            this.db = db;
        }

        public void Update(Company obj)
        {
            db.Companies.Update(obj);
        }

      
    }
}
