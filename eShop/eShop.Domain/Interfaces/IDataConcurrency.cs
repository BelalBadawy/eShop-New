
namespace eShop.Domain.Interfaces
{
    public interface IDataConcurrency
    {
        [Timestamp]
        public byte[] RowVersion { get; set; }

    }
}
