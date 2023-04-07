using Microsoft.EntityFrameworkCore;

namespace TestCodEjemplo
{
    public interface IRepository<TEntity, TContext>
        where TEntity : class
        where TContext : DbContext
    {
    }
}