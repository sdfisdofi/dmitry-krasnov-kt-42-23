using dmitry_krasnov_kt_42_23.Database.Helpers;
using dmitry_krasnov_kt_42_23.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace dmitry_krasnov_kt_42_23.Database.Configurations
{
    public class CreditConfiguration : IEntityTypeConfiguration<Credit>
    {
        private const string TableName = "cd_credit";

        public void Configure(EntityTypeBuilder<Credit> builder)
        {
            builder
                .HasKey(p => p.CreditId)
                .HasName($"pk_{TableName}_credit_id");

            builder.Property(p => p.CreditId)
                .ValueGeneratedOnAdd();

            builder.Property(p => p.CreditId)
                .HasColumnName("credit_id")
                .HasComment("Идентификатор записи зачета");

            builder.Property(p => p.IsPassed)
                .IsRequired()
                .HasColumnName("b_credit_is_passed")
                .HasColumnType(ColumnType.Bool)
                .HasComment("Зачет сдан (true) / не сдан (false)");

            builder.Property(p => p.Date)
                .IsRequired()
                .HasColumnName("d_credit_date")
                .HasColumnType(ColumnType.Date)
                .HasComment("Дата сдачи зачета");

            builder.Property(p => p.StudentId)
                .IsRequired()
                .HasColumnName("f_student_id")
                .HasComment("Идентификатор студента");

            builder.Property(p => p.DisciplineId)
                .IsRequired()
                .HasColumnName("f_discipline_id")
                .HasComment("Идентификатор дисциплины");

            builder.ToTable(TableName)
                .HasOne(p => p.Student)
                .WithMany()
                .HasForeignKey(p => p.StudentId)
                .HasConstraintName("fk_credit_f_student_id")
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable(TableName)
                .HasOne(p => p.Discipline)
                .WithMany()
                .HasForeignKey(p => p.DisciplineId)
                .HasConstraintName("fk_credit_f_discipline_id")
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
