using CQRSPattern.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CQRSPattern.Infrastructure.Persistence.Configuration;

public class EmployeeEntityConfiguration : IEntityTypeConfiguration<EmployeeEntity>
{
    public void Configure(EntityTypeBuilder<EmployeeEntity> builder)
    {
        builder.ToTable(Application.Constants.Database.TableEmployee);

        builder.HasKey(e => e.Id).HasName("Employee_PK");

        builder.Property(e => e.BirthDate).IsRequired();

        builder.Property(e => e.FirstName).HasMaxLength(30).IsRequired();

        builder.Property(e => e.LastName).HasMaxLength(30).IsRequired();

        builder.Property(e => e.Gender).HasMaxLength(1).IsRequired();

        builder.Property(e => e.HireDate).IsRequired();
    }
}
