using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetworkScanner.Database.Entities;

namespace NetworkScanner.Database.Configurations
{
    public class HostConfiguration : IEntityTypeConfiguration<HostEntity>
    {
        public void Configure(EntityTypeBuilder<HostEntity> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(b => b.Address)
                .IsRequired();
        }
    }
}
