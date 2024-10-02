using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Bulky.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        // T - Category

        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter =null,string? includeProperties = null);
        /// <summary>
        /// Gets the Category based on Id
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        T Get(Expression<Func<T,bool>> filter, string? includeProperties = null, bool tracked =false);

        void Add(T entity);
        void Delete(T entity);
       
        // Takes list of entity to delete
        void DeleteRange(IEnumerable<T> entities);
    }
}
