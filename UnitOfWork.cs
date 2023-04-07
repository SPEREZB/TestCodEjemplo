using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
namespace TestCodEjemplo
{
     

    public class UnitOfWork : IUnitOfWork
    {
        private readonly Dictionary<Type, DbContext> _dbContexts;
        private readonly Dictionary<Type, DbContextTransaction> _transactions;

        public IHttpContextAccessor HttpContextAccessor { get; internal set; }

        public UnitOfWork(Dictionary<Type, DbContext> dbContexts)
        {
            _dbContexts = dbContexts;
            _transactions = new Dictionary<Type, DbContextTransaction>();
        }

        public void BeginTransaction<TContext>() where TContext : DbContext
        {
            if (!_dbContexts.ContainsKey(typeof(TContext)))
            {
                throw new InvalidOperationException($"El contexto de tipo {typeof(TContext).Name} no ha sido registrado.");
            }

            if (_transactions.ContainsKey(typeof(TContext)))
            {
                throw new InvalidOperationException($"La transacción para el contexto de tipo {typeof(TContext).Name} ya ha sido iniciada.");
            }
            //SE CORRIGIO LA LINEA 30 DEBIDO A QUE ESTABA INTENTADO CONVERTIR DEL TIPO Microsoft.EntityFrameworkCore.Storage.IdbContextTransaction EN
            //TestCodEjemplo.DbContextTransaction
            //PARA CORREGIR EL ERROR SIMPLEMENTE SE AGREGO  (DbContextTransaction) PARA HACER UNA CONVERSION EXPLICITA  
            _transactions[typeof(TContext)] = (DbContextTransaction)_dbContexts[typeof(TContext)].Database.BeginTransaction();
      }

        public void PruebaBeginTransaction()
        {

            throw new InvalidOperationException();
        }

        public void Commit()
        {
            try
            {
                foreach (var transaction in _transactions.Values)
                {
                    transaction?.Commit();
                }
            }
            catch
            {
                Rollback();
                throw;
            }
            finally
            {
                Dispose();
            }
        }

        public void Rollback()
        {
            foreach (var transaction in _transactions.Values)
            {
                transaction?.Rollback();
                transaction?.Dispose();
            }
            _transactions.Clear();
        }

        public IRepository<TEntity, TContext> GetRepository<TEntity, TContext>()
            where TEntity : class
            where TContext : DbContext
        {
            if (!_dbContexts.ContainsKey(typeof(TContext)))
            {
                throw new InvalidOperationException($"El contexto de tipo {typeof(TContext).Name} no ha sido registrado.");
            }

            return new Repository<TEntity, TContext>(_dbContexts[typeof(TContext)]);
        }

        public void Dispose()
        {
            foreach (var context in _dbContexts.Values)
            {
                context?.Dispose();
            }

            foreach (var transaction in _transactions.Values)
            {
                transaction?.Dispose();
            }
        }



        // Método GetOk() que retorna un IActionResult
        public IActionResult GetOk()
        {
            // Implementa la lógica de tu método GetOk() aquí, que retorna un IActionResult
            // En este ejemplo, se retorna un OkObjectResult con un valor de ejemplo "Resultado Ok"
            return new OkObjectResult("Resultado Ok");
        }
    }

    public interface IUnitOfWork : IDisposable
    {
        void BeginTransaction<TContext>() where TContext : DbContext;
        void Commit();
        void Rollback();
        IRepository<TEntity, TContext> GetRepository<TEntity, TContext>()
        where TEntity : class
        where TContext : DbContext;
    }



    // DEFINIMOS NUESTRO DbContextTransaction
    // SE AGREGO CODIGO FALTANTE LOS METODOS COMMIT Y ROLLBACK
    public class DbContextTransaction : DbContext
    { 

        public DbContextTransaction()
        {
        }

        public DbContextTransaction(DbContextOptions<DbContextTransaction> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }

        public void Commit()
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }
          public void pruebarollback()
    {

    }
    }
     

}
