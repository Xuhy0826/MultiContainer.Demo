using System.Collections.Generic;
using Domain.Abstractions;
using Order.Aggregate.OrderAggregate;

namespace Demo.Order.Domain.Events
{
    /// <summary>
    /// 
    /// </summary>
    public class OrderStatusChangedToAwaitingValidationDomainEvent : IDomainEvent
    {
        public int OrderId { get; }
        public IEnumerable<OrderItem> OrderItems { get; }

        public OrderStatusChangedToAwaitingValidationDomainEvent(int orderId,
            IEnumerable<OrderItem> orderItems)
        {
            OrderId = orderId;
            OrderItems = orderItems;
        }
    }
}
