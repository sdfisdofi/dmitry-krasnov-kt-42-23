using dmitry_krasnov_kt_42_23.Database.Helpers;
using dmitry_krasnov_kt_42_23.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dmitry_krasnov_kt_42_23.Database.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        // Название таблицы, которое будет отображаться в БД
        private const string TableName = "cd_group";

        public void Configure(EntityTypeBuilder<Group> builder)
        {
            // Задаем первичный ключ
            builder
                .HasKey(p => p.GroupId)
                .HasName($"pk_{TableName}_group_id");

            // Для целочисленного первичного ключа задаем автогенерацию
            builder.Property(p => p.GroupId)
                .ValueGeneratedOnAdd();

            // Расписываем как будут называться колонки в БД, а также их обязательность и тд
            builder.Property(p => p.GroupId)
                .HasColumnName("group_id")
                .HasComment("Идентификатор записи группы");

            builder.Property(p => p.GroupName)
                .IsRequired()
                .HasColumnName("c_group_name")
                .HasColumnType(ColumnType.String).HasMaxLength(100)
                .HasComment("Название группы");

            builder.Property(p => p.Specialty)
                .IsRequired()
                .HasColumnName("c_group_specialty")
                .HasColumnType(ColumnType.String).HasMaxLength(200)
                .HasComment("Специальность группы");

            builder.Property(p => p.Year)
                .IsRequired()
                .HasColumnName("n_group_year")
                .HasColumnType(ColumnType.Int)
                .HasComment("Год набора группы");

            builder.Property(p => p.IsDeleted)
                .IsRequired()
                .HasColumnName("b_is_deleted")
                .HasColumnType(ColumnType.Bool)
                .HasComment("Признак удаления группы");

            builder.ToTable(TableName);
        }
    }
}
