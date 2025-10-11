namespace eShop.Domain.Events
{
    public class BrandCreatedEvent : IDomainEvent
    {
        public Guid BrandId { get; }
        public BrandCreatedEvent(Guid id)
        {
            BrandId = id;
        }
    }
}
