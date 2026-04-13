using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Models;
using Domain.Models.OrderModels;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;

namespace Persistence.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        private readonly RafiqDbContext context;

        public GenericRepository(RafiqDbContext context)
        {
            this.context = context;
        }
        public async Task<IEnumerable<TEntity>> GetAllAsyns(bool trackchanges = false)
        {
            return trackchanges ?
                  await context.Set<TEntity>().ToListAsync()
                : await context.Set<TEntity>().AsNoTracking().ToListAsync();

            //if (trackchanges) return await context.Set<TEntity>().ToListAsync();
            //return await context.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public async Task<TEntity?> GetAsyns(TKey id)
        {
            return await context.Set<TEntity>().FindAsync(id);
        }

        public async Task AddAsyns(TEntity entity)
        {
            await context.AddAsync(entity);
        }
        public void Update(TEntity entity)
        {
            context.Update(entity);
        }

        public void Delete(TEntity entity)
        {
            context.Remove(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsyns(ISpecifications<TEntity, TKey> specifications, bool trackchanges = false)
        {
          return await ApplySpcifications(specifications).ToListAsync();
        }

        public async Task<TEntity?> GetAsyns(ISpecifications<TEntity, TKey> specifications)
        {
          return await ApplySpcifications(specifications).FirstOrDefaultAsync();
        }

        public async Task<int> CountAsync(ISpecifications<TEntity, TKey> specifications)
        {
            return await ApplySpcifications(specifications).CountAsync();
        }
        private IQueryable<TEntity> ApplySpcifications(ISpecifications<TEntity,TKey> specifications) 
        {
            return SpecificationEvaluator.GetQuery(context.Set<TEntity>(), specifications);
        }

      
    }
}
