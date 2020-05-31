using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCore.CAP;

namespace Demo.Catalog.API.IntegrationEvents
{
    public class CatalogIntegrationEventService : ICatalogIntegrationEventService, ICapSubscribe
    {
        //[CapSubscribe("OrderCreated")]
        //public void OrderCreated(OrderCreatedIntegrationEvent @event)
        //{
        //    //Do SomeThing
        //}
    }
}
