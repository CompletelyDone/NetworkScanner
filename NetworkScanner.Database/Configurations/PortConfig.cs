using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetworkScanner.Model.Models;

namespace NetworkScanner.Database.Configurations
{
    public class PortConfig : IEntityTypeConfiguration<Port>
    {
        public void Configure(EntityTypeBuilder<Port> builder)
        {
            builder.HasKey(x=>x.Number);

            builder.HasOne(x => x.Host).WithMany(x => x.Ports);
        }
    }
}
