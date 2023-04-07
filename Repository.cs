using Microsoft.EntityFrameworkCore;

namespace TestCodEjemplo
{
    public class Repository<TEntity, TContext> : IRepository<TEntity, TContext>
        where TEntity : class
        where TContext : DbContext
    {
        private DbContext dbContext;

        public Repository(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }
    }
}