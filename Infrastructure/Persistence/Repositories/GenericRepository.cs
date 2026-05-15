using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Models;
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

        public async Task<IEnumerable<TEntity>> GetAllAsync(bool trackchanges = false)
        {
            return trackchanges ?
                  await context.Set<TEntity>().ToListAsync()
                : await context.Set<TEntity>().AsNoTracking().ToListAsync();
        }

        public async Task<TEntity?> GetAsync(TKey id)
        {
            return await context.Set<TEntity>().FindAsync(id);
        }

        public async Task AddAsync(TEntity entity)
        {
            await context.AddAsync(entity);
        }

        public void Update(TEntity entity)
        {
            context.Update(entity);
        }

        public async void Delete(int id)
        {
            var result = await context.FindAsync<TEntity>(id);
            context.Remove(result);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> specifications, bool trackchanges = false)
        {
          return await ApplySpcifications(specifications).ToListAsync();
        }

        public async Task<TEntity?> GetAsync(ISpecifications<TEntity, TKey> specifications)
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

        public async Task<TEntity?> FindAsync(TKey id)
        {
            return await context.Set<TEntity>().FindAsync(id);
        }

        public async Task<IEnumerable<TEntity>> GetAllWithIncludesAsync(Expression<Func<TEntity, bool>> criteria = null, params string[] includes)
        {
            IQueryable<TEntity> query = context.Set<TEntity>();

            // 1. تنفيذ الـ Includes "على نضافة"
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            // 2. تنفيذ الـ Filter (الـ Criteria) لو موجودة
            if (criteria != null)
            {
                query = query.Where(criteria);
            }

            return await query.ToListAsync();

        }

        public Task<IQueryable<TEntity>> GetAllAsQueryable()
        {
            return context.Set<TEntity>().AsNoTracking().ToListAsync().ContinueWith(t => t.Result.AsQueryable());
        }

        public async Task<Appointment> GetByIdAsync(int appointmentId)
        {
            return await context.Set<Appointment>().FindAsync(appointmentId);
        }
    }
}
