using dmitry_krasnov_kt_42_23.Database.Helpers;
using dmitry_krasnov_kt_42_23.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dmitry_krasnov_kt_42_23.Database.Configurations
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        // Название таблицы, которое будет отображаться в БД
        private const string TableName = "cd_student";

        public void Configure(EntityTypeBuilder<Student> builder)
        {
            // Задаем первичный ключ
            builder
                .HasKey(p => p.StudentId)
                .HasName($"pk_{TableName}_student_id");

            // Для целочисленного первичного ключа задаем автогенерацию (к каждой новой записи будет добавлять +1)
            builder.Property(p => p.StudentId)
                .ValueGeneratedOnAdd();

            // Расписываем как будут называться колонки в БД, а также их обязательность и тд
            builder.Property(p => p.StudentId)
                .HasColumnName("student_id")
                .HasComment("Идентификатор записи студента");

            builder.Property(p => p.FirstName)
                .IsRequired()
                .HasColumnName("c_student_firstname")
                .HasColumnType(ColumnType.String).HasMaxLength(100)
                .HasComment("Имя студента");

            builder.Property(p => p.LastName)
                .IsRequired()
                .HasColumnName("c_student_lastname")
                .HasColumnType(ColumnType.String).HasMaxLength(100)
                .HasComment("Фамилия студента");

            // Отчество необязательное (без IsRequired)
            builder.Property(p => p.MiddleName)
                .HasColumnName("c_student_middlename")
                .HasColumnType(ColumnType.String).HasMaxLength(100)
                .HasComment("Отчество студента");

            builder.Property(p => p.GroupId)
                .IsRequired()
                .HasColumnName("f_group_id")
                .HasComment("Идентификатор группы");

            builder.Property(p => p.IsDeleted)
                .IsRequired()
                .HasColumnName("b_is_deleted")
                .HasColumnType(ColumnType.Bool)
                .HasComment("Признак удаления студента");

            // Связь: у Студента одна Группа (HasOne), у Группы много Студентов (WithMany)
            // Удаление у нас мягкое (IsDeleted), поэтому физически удалять группу со студентами БД запрещаем (Restrict)
            builder.ToTable(TableName)
                .HasOne(p => p.Group)
                .WithMany()
                .HasForeignKey(p => p.GroupId)
                .HasConstraintName("fk_f_group_id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable(TableName)
                .HasIndex(p => p.GroupId, $"idx_{TableName}_fk_f_group_id");

            // Добавим явную автоподгрузку связанной сущности
            builder.Navigation(p => p.Group)
                .AutoInclude();
        }
    }
}
