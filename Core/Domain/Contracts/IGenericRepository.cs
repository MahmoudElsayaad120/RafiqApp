using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Domain.Contracts
{
    public interface IGenericRepository<TEntity ,TKey> where TEntity : BaseEntity<TKey>
    {
        Task<IEnumerable<TEntity>> GetAllAsyns(bool trackchanges = false);
        Task<TEntity?> GetAsyns(TKey id);
        Task AddAsyns(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
