using eShopAnalysis.CartOrderAPI.Domain.SeedWork;
using eShopAnalysis.CartOrderAPI.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace eShopAnalysis.CartOrderAPI.Infrastructure
{

    public class UnitOfWork : IUnitOfWork
    {
        private ICartRepository _cartRepository;
        public IOrderRepository _orderRepository;
        private readonly OrderCartContext _orderCartContext;
        private IDbContextTransaction _currentTransaction;
        private IMediator _mediator;
        public UnitOfWork(OrderCartContext orderCartContext, IMediator mediator)
        {
            _orderCartContext = orderCartContext;
            _mediator = mediator;
        }

        public IOrderRepository OrderRepository
        {
            get
            {
                if (_orderRepository == null)
                {
                    _orderRepository = new OrderRepository(this._orderCartContext);
                }
                return _orderRepository;
            }
        }

        public ICartRepository CartRepository
        {
            get
            {
                if (_cartRepository == null)
                {
                    _cartRepository = new CartRepository(this._orderCartContext);
                }
                return _cartRepository;
            }
        }

        public bool HasActiveTransaction() => _currentTransaction != null;

        public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;

        public void Dispose()
        {
            _orderCartContext.Dispose();
            GC.SuppressFinalize(this);
        }


        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            if (_currentTransaction != null) return null;
            _currentTransaction = await _orderCartContext.Database.BeginTransactionAsync();
            return _currentTransaction;
        }

        public async Task CommitTransactionAsync(IDbContextTransaction transaction)
        {
            if (transaction == null) return;
            try
            {
                //immediate consistency publish all domain event here
                await PublishAllPendingDomainEvents();
                await this._orderCartContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public void RollbackTransaction()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        private async Task PublishAllPendingDomainEvents()
        {
            var allDomainEvents = _orderCartContext.ChangeTracker.Entries<Entity>()
                .Select(entry => entry.Entity)
                .SelectMany(entity => {
                    var allThisEntityDomainEvents = entity.GetDomainEvents();
                    entity.ClearDomainEvents();
                    return allThisEntityDomainEvents;
                })
                .ToList();

            foreach (var domainEvent in allDomainEvents) {
                await _mediator.Publish(domainEvent); //all domain events will be handle, only after all are done, we mark this as done
            }
        }
    }
}
