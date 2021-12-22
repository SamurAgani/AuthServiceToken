using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Repositories
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        public Task<TEntity> GetByIdAsync(int id);
        public Task<IEnumerable<TEntity>> GetAllAsync();
        public IQueryable<TEntity> Where(Expression<Func<TEntity,bool>> predicate);
        public Task AddAsync(TEntity entity);
        public void Remove(TEntity entity);
        public TEntity Update(TEntity entity);

    }
}
