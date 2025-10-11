namespace eShop.Domain.Interfaces
{
    public interface ISoftDelete
    {
        public bool SoftDeleted { get; set; }

        public int? DeletedBy { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DeletedAt { get; set; }
    }
}
