using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Abstractions
{
    public abstract class Entity : IEntity
    {
        public abstract object[] Keys { get; }

        public override string ToString()
        {
            return $"[Entity: {GetType().Name}] Keys = {string.Join(",", Keys)}";
        }

        #region 领域事件
        private List<IDomainEvent> _domainEvents;
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();

        public void AddDomainEvent(IDomainEvent eventItem)
        {
            _domainEvents = _domainEvents ?? new List<IDomainEvent>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(IDomainEvent eventItem)
        {
            _domainEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            _domainEvents?.Clear();
        }
        #endregion
    }

    public abstract class Entity<TKey> : Entity, IEntity<TKey>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual TKey Id { get; protected set; }
        public override object[] Keys => new object[]{Id};

        public override bool Equals(object obj)
        {
            if (!(obj is Entity<TKey>))
                return false;

            if (object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            var item = (Entity<TKey>)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            return item.Id.Equals(this.Id);
        }

        private int? _requestedHashCode;
        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = this.Id.GetHashCode() ^ 31;
                return _requestedHashCode.Value;
            }
            else
                return base.GetHashCode();
        }

        //表示对象是否为全新创建的，未持久化的
        public bool IsTransient()
        {
            return EqualityComparer<TKey>.Default.Equals(Id, default);
        }
        public override string ToString()
        {
            return $"[Entity: {GetType().Name}] Id = {Id}";
        }
        public static bool operator ==(Entity<TKey> left, Entity<TKey> right)
        {
            if (object.Equals(left, null))
                return (object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
        {
            return !(left == right);
        }
    }
}
