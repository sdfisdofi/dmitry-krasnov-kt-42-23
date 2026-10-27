using dmitry_krasnov_kt_42_23.Database.Helpers;
using dmitry_krasnov_kt_42_23.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dmitry_krasnov_kt_42_23.Database.Configurations
{
    public class SpecialityConfiguration : IEntityTypeConfiguration<Speciality>
    {
        // Название таблицы, которое будет отображаться в БД
        private const string TableName = "cd_speciality";

        public void Configure(EntityTypeBuilder<Speciality> builder)
        {
            // Задаем первичный ключ
            builder
                .HasKey(p => p.SpecialityId)
                .HasName($"pk_{TableName}_speciality_id");

            // Для целочисленного первичного ключа задаем автогенерацию
            builder.Property(p => p.SpecialityId)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.SpecialityId)
                .HasColumnName("speciality_id")
                .HasComment("Идентификатор записи специальности");

            builder.Property(p => p.Title)
                .IsRequired()
                .HasColumnName("c_speciality_title")
                .HasColumnType(ColumnType.String).HasMaxLength(200)
                .HasComment("Название специальности");

            // Код необязательный (без IsRequired)
            builder.Property(p => p.Code)
                .HasColumnName("c_speciality_code")
                .HasColumnType(ColumnType.String).HasMaxLength(20)
                .HasComment("Код специальности");

            builder.Property(p => p.IsDeleted)
                .IsRequired()
                .HasColumnName("b_is_deleted")
                .HasColumnType(ColumnType.Bool)
                .HasComment("Признак удаления специальности");

            builder.ToTable(TableName);
        }
    }
}