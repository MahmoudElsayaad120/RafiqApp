using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contracts;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Persistence
{
    static class SpecificationEvaluator
    {
        public static IQueryable<TEntity> GetQuery<TEntity, TKey>(
            IQueryable<TEntity> inputQuery, 
            ISpecifications<TEntity,TKey> specifications
            ) 
            where TEntity : BaseEntity<TKey>
        {
            var query = inputQuery;

            if (specifications.Criteria is not null)
              query =  query.Where(specifications.Criteria);

            if (specifications.IsPageination)
                query = query.Skip(specifications.Skip).Take(specifications.Take);

            query = specifications.IncludeExpression.Aggregate(query, (currentQuery, IncludeExpression) => currentQuery.Include(IncludeExpression));


            return query;
        }
    }
}
