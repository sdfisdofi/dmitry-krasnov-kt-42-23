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

            builder.Property(p => p.SpecialityId)
                .IsRequired()
                .HasColumnName("f_speciality_id")
                .HasComment("Идентификатор специальности");

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

            // Связь: у Группы одна Специальность (HasOne), у Специальности много Групп (WithMany)
            // Удаление у нас мягкое (IsDeleted), поэтому физически удалять специальность с группами БД запрещаем (Restrict)
            builder.ToTable(TableName)
                .HasOne(p => p.Speciality)
                .WithMany()
                .HasForeignKey(p => p.SpecialityId)
                .HasConstraintName("fk_f_speciality_id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable(TableName)
                .HasIndex(p => p.SpecialityId, $"idx_{TableName}_fk_f_speciality_id");

            // Специальность подгружается вместе с группой автоматически
            builder.Navigation(p => p.Speciality)
                .AutoInclude();
        }
    }
}
