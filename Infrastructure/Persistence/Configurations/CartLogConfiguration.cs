using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public class CartLogConfiguration : IEntityTypeConfiguration<CartLog>
    {
        public void Configure(EntityTypeBuilder<CartLog> builder)
        {
            builder.ToTable("CartLogs");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Folio)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            builder.Property(x => x.CartTypeId)
                .HasColumnName("cartTypeId");

            builder.HasOne(x => x.CartType)
                .WithMany(t => t.CartLogs)
                .HasForeignKey(x => x.CartTypeId);

            builder.Property(x => x.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(x => x.EntryDate)
                .HasDefaultValueSql("GETDATE()");
        }
    }
}