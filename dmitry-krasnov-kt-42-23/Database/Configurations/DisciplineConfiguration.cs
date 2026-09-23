using dmitry_krasnov_kt_42_23.Database.Helpers;
using dmitry_krasnov_kt_42_23.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dmitry_krasnov_kt_42_23.Database.Configurations
{
    public class DisciplineConfiguration : IEntityTypeConfiguration<Discipline>
    {
        private const string TableName = "cd_discipline";

        public void Configure(EntityTypeBuilder<Discipline> builder)
        {
            builder
                .HasKey(p => p.DisciplineId)
                .HasName($"pk_{TableName}_discipline_id");

            builder.Property(p => p.DisciplineId)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.DisciplineId)
                .HasColumnName("discipline_id")
                .HasComment("Идентификатор записи дисциплины");

            builder.Property(p => p.DisciplineName)
                .IsRequired()
                .HasColumnName("c_discipline_name")
                .HasColumnType(ColumnType.String).HasMaxLength(200)
                .HasComment("Название дисциплины");

            // enum хранится в БД как число: 1 - гуманитарное, 2 - техническое
            builder.Property(p => p.Direction)
                .IsRequired()
                .HasColumnName("n_discipline_direction")
                .HasColumnType(ColumnType.Int)
                .HasComment("Направление дисциплины (1 - гуманитарное, 2 - техническое)");

            builder.Property(p => p.IsDeleted)
                .IsRequired()
                .HasColumnName("b_is_deleted")
                .HasColumnType(ColumnType.Bool)
                .HasComment("Признак удаления дисциплины");

            builder.ToTable(TableName);
        }
    }
}
