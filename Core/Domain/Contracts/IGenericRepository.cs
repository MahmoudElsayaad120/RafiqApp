using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Contracts
{
    public interface IGenericRepository<TEntity ,TKey> where TEntity : BaseEntity<TKey>
    {
        //IEnumerable<object> Table { get; set; }

        Task<int> CountAsync(ISpecifications<TEntity, TKey> specifications);
        Task<IEnumerable<TEntity>> GetAllAsync(bool trackchanges = false);
        Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> specifications, bool trackchanges = false);
        Task<TEntity?> GetAsync(TKey id);
        Task<TEntity?> GetAsync(ISpecifications<TEntity, TKey> specifications);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(int id);
        Task<TEntity?> FindAsync(TKey id);
        Task<IEnumerable<TEntity>> GetAllWithIncludesAsync(
            Expression<Func<TEntity, bool>> criteria = null,
            params string[] includes);
        Task<IQueryable<TEntity>> GetAllAsQueryable();
        Task<Appointment> GetByIdAsync(int appointmentId);
    }
}
