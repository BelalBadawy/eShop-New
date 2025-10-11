namespace eShop.Domain.Common
{
    public abstract class BaseEntity<TId> : IEntity<TId> where TId : notnull
    {

        [Key]
        public TId Id { get; set; }

        private readonly List<IDomainEvent> _domainEvents = new();

        [NotMapped]
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        public List<IDomainEvent> PopDomainEvents()
        {
            var copy = _domainEvents.ToList();
            ClearDomainEvents();
            return copy;
        }

        public void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void RemoveDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
