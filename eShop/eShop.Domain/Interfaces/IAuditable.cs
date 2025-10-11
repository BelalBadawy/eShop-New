namespace eShop.Domain.Interfaces
{
    public interface IAuditable
    {
        public int? CreatedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }

        public int? LastModifiedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? LastModifiedAt { get; set; }
    }
}
