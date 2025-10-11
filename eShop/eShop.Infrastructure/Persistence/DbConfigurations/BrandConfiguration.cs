using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eShop.Infrastructure.Persistence.DbConfigurations
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            // Table
            builder.ToTable("Brands");

            // Primary Key with Auto Increment
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("int");

            // Title
            builder.Property(b => b.Title)
                .IsRequired()
                .HasMaxLength(124)
                .HasColumnType("nvarchar(124)");

            // Soft delete
            builder.Property(b => b.SoftDeleted)
                .IsRequired()
                .HasDefaultValue(false)
                .HasColumnType("bit");

            builder.Property(b => b.DeletedBy)
                .IsRequired(false)
                .HasColumnType("int");

            builder.Property(b => b.DeletedAt)
                .IsRequired(false)
                .HasColumnType("datetime2");

            // Concurrency token
            builder.Property(b => b.RowVersion)
                .IsRowVersion()
                .IsConcurrencyToken()
                .HasColumnType("rowversion");

            // Audit fields
            builder.Property(b => b.CreatedBy)
                .IsRequired(false)
                .HasColumnType("int");

            builder.Property(b => b.CreatedAt)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(b => b.LastModifiedBy)
                .IsRequired(false)
                .HasColumnType("int");

            builder.Property(b => b.LastModifiedAt)
                .IsRequired(false)
                .HasColumnType("datetime2");

        }
    }
}
