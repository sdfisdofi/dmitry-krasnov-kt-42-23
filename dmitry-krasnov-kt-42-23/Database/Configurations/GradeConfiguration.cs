using dmitry_krasnov_kt_42_23.Database.Helpers;
using dmitry_krasnov_kt_42_23.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dmitry_krasnov_kt_42_23.Database.Configurations
{
    public class GradeConfiguration : IEntityTypeConfiguration<Grade>
    {
        private const string TableName = "cd_grade";

        public void Configure(EntityTypeBuilder<Grade> builder)
        {
            builder
                .HasKey(p => p.GradeId)
                .HasName($"pk_{TableName}_grade_id");

            builder.Property(p => p.GradeId)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.GradeId)
                .HasColumnName("grade_id")
                .HasComment("Идентификатор записи оценки");

            builder.Property(p => p.Value)
                .IsRequired()
                .HasColumnName("n_grade_value")
                .HasColumnType(ColumnType.Int)
                .HasComment("Оценка (от 2 до 5)");

            builder.Property(p => p.Date)
                .IsRequired()
                .HasColumnName("d_grade_date")
                .HasColumnType(ColumnType.Date)
                .HasComment("Дата выставления оценки");

            builder.Property(p => p.StudentId)
                .IsRequired()
                .HasColumnName("f_student_id")
                .HasComment("Идентификатор студента");

            builder.Property(p => p.DisciplineId)
                .IsRequired()
                .HasColumnName("f_discipline_id")
                .HasComment("Идентификатор дисциплины");

            // Связь с таблицей студентов: у оценки один студент, у студента много оценок
            builder.ToTable(TableName)
                .HasOne(p => p.Student)
                .WithMany()
                .HasForeignKey(p => p.StudentId)
                .HasConstraintName("fk_grade_f_student_id")
                .OnDelete(DeleteBehavior.Restrict);

            // Связь с таблицей дисциплин
            builder.ToTable(TableName)
                .HasOne(p => p.Discipline)
                .WithMany()
                .HasForeignKey(p => p.DisciplineId)
                .HasConstraintName("fk_grade_f_discipline_id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable(TableName)
                .HasIndex(p => p.StudentId, $"idx_{TableName}_fk_f_student_id");

            builder.ToTable(TableName)
                .HasIndex(p => p.DisciplineId, $"idx_{TableName}_fk_f_discipline_id");

            builder.Navigation(p => p.Student)
                .AutoInclude();

            builder.Navigation(p => p.Discipline)
                .AutoInclude();
        }
    }
}
