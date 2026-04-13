using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Domain.Models.OrderModels;

namespace Domain.Contracts
{
    public interface IGenericRepository<TEntity ,TKey> where TEntity : BaseEntity<TKey>
    {

        Task<int> CountAsync(ISpecifications<TEntity, TKey> specifications);
        Task<IEnumerable<TEntity>> GetAllAsyns(bool trackchanges = false);
        Task<IEnumerable<TEntity>> GetAllAsyns(ISpecifications<TEntity, TKey> specifications, bool trackchanges = false);
        Task<TEntity?> GetAsyns(TKey id);
        Task<TEntity?> GetAsyns(ISpecifications<TEntity, TKey> specifications);
        Task AddAsyns(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        
    }
}
