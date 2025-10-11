namespace eShop.Domain.Entities
{
    public class Brand : BaseEntity<int>, IFullEntity
    {
        public string Title { get; set; } = string.Empty;
        public bool SoftDeleted { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
        public int? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? LastModifiedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }
    }
}
