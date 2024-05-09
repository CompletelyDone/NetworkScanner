using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetworkScanner.Model.Models;

namespace NetworkScanner.Database.Configurations
{
    public class HostConfig : IEntityTypeConfiguration<Host>
    {
        public void Configure(EntityTypeBuilder<Host> builder)
        {
            builder.HasKey(x=>x.Id);

            builder.HasMany(x => x.Ports).WithOne(x => x.Host);

            builder.Property(h=>h.IPAddress)
                .IsRequired();
            builder.Property(h => h.PacketsSend)
                .IsRequired();
            builder.Property(h => h.PacketsReceived)
                .IsRequired();
        }
    }
}
