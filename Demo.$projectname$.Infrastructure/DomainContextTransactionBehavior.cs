﻿using DotNetCore.CAP;
using Infrastructure.Core.Behaviors;

namespace Demo.$projectname$.Infrastructure
{
    public class DomainContextTransactionBehavior<TRequest, TResponse> : TransactionBehavior<DomainContext, TRequest, TResponse>
    {
        public DomainContextTransactionBehavior(DomainContext dbContext, ICapPublisher capBus)
            : base(dbContext, capBus)
        {
        }
    }
}
