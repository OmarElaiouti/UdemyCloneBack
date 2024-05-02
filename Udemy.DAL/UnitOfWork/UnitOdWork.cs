using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.DAL.UnitOfWork
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Storage;
    using System;
    using System.Threading.Tasks;

    public class UnitOfWork<TContext> : IUnitOfWork<TContext>, IDisposable where TContext : DbContext
    {
        private readonly TContext _context;
        private IDbContextTransaction? _transaction;
        private bool _disposed = false;

        public UnitOfWork(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public TContext Context => _context;

        public async Task CreateTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Transaction has not been started.");
            }

            await _transaction.CommitAsync();
        }

        public async Task RollbackAsync()
        {
            if (_transaction == null)
            {
                throw new InvalidOperationException("Transaction has not been started.");
            }

            await _transaction.RollbackAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        // Implement IDisposable
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Release managed resources
                    _transaction?.Dispose();
                    _context?.Dispose();
                }

                // Release unmanaged resources
                _disposed = true;
            }
        }

        
    }

}
