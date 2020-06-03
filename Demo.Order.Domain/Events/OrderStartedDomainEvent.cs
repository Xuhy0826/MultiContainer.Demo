using System;
using Domain.Abstractions;

namespace Demo.Order.Domain.Events
{
    /// <summary>
    /// 创建订单后的领域事件
    /// </summary>
    public class OrderStartedDomainEvent : IDomainEvent
    {
        public string UserId { get; }
        public string UserName { get; }
        public int CardTypeId { get; }
        public string CardNumber { get; }
        public string CardSecurityNumber { get; }
        public string CardHolderName { get; }
        public DateTime CardExpiration { get; }
        public global::Order.Aggregate.OrderAggregate.Order Order { get; }

        public OrderStartedDomainEvent(global::Order.Aggregate.OrderAggregate.Order order, string userId, string userName,
            int cardTypeId, string cardNumber,
            string cardSecurityNumber, string cardHolderName,
            DateTime cardExpiration)
        {
            Order = order;
            UserId = userId;
            UserName = userName;
            CardTypeId = cardTypeId;
            CardNumber = cardNumber;
            CardSecurityNumber = cardSecurityNumber;
            CardHolderName = cardHolderName;
            CardExpiration = cardExpiration;
        }
    }
}
