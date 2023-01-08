using Friendbook.DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Friendbook.DataAccessLayer.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.ToTable("People");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.FirstName).HasMaxLength(30).IsRequired();
        builder.Property(x => x.LastName).HasMaxLength(30).IsRequired();
        builder.Property(x => x.City).HasMaxLength(50);
        builder.Property(x => x.CreatedBy).HasMaxLength(512);
    }
}