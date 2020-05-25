using System;
using System.Threading;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Mark.Common.ExtensionMethod;
using Mark.Common.Helper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Core.Behaviors
{
    //IPipelineBehavior为MediatoR的事件增加管道流程，在Handle方法中的next标识触发的MediatoR事件
    public class TransactionBehavior<TDbContext, TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
        where TDbContext : EFContext
    {
        TDbContext _dbContext;
        ICapPublisher _capBus;
        public TransactionBehavior(TDbContext dbContext, ICapPublisher capBus)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _capBus = capBus ?? throw new ArgumentNullException(nameof(capBus));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var typeName = request.GetGenericTypeName();
            try
            {
                //如果事务开启了的话，我们直接处理事件
                if (_dbContext.HasActiveTransaction)
                {
                    return await next();
                }
                //否则我们开启一个事务
                var strategy = _dbContext.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    Guid transactionId;
                    using var transaction = await _dbContext.BeginTransactionAsync();
                    Logger.Info("TransactionContext:{TransactionId}", transaction.TransactionId);
                    Logger.Info("开始事务 {TransactionId} ({@Command})", transaction.TransactionId, typeName, request);

                    response = await next();

                    Logger.Info("提交事务 {TransactionId} {CommandName}", transaction.TransactionId, typeName);

                    await _dbContext.CommitTransactionAsync(transaction);

                    transactionId = transaction.TransactionId;
                });
                return response;
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "处理事务出错 {CommandName} ({@Command})", typeName, request);
                throw;
            }
        }
    }
}
