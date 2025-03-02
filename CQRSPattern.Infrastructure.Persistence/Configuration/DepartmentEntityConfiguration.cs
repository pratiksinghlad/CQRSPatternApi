using CQRSPattern.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CQRSPattern.Infrastructure.Persistence.Configuration;

public class DepartmentEntityConfiguration : IEntityTypeConfiguration<DepartmentEntity>
{
    public void Configure(EntityTypeBuilder<DepartmentEntity> builder)
    {
        builder.ToTable(Application.Constants.Database.TableDepartment);

        builder.HasKey(e => e.Id).HasName("Department_PK");

        builder.Property(e => e.Name)
            .HasMaxLength(30)
            .IsRequired();
    }
}