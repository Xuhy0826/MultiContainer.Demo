using Domain.Abstractions;

namespace Demo.Order.Domain.Events
{
    public class OrderStatusChangedToStockConfirmedDomainEvent : IDomainEvent
    {
        public int OrderId { get; }

        public OrderStatusChangedToStockConfirmedDomainEvent(int orderId)
            => OrderId = orderId;
    }
}
