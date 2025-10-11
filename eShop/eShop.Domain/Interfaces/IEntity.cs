namespace eShop.Domain.Interfaces
{
    public interface IEntity<TId> : IEntity
    {
        TId Id { get; protected set; }
    }
    public interface IEntity
    {

    }
}
