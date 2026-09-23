using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dmitry_krasnov_kt_42_23.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_f_group_id",
                table: "cd_student");

            migrationBuilder.AlterColumn<string>(
                name: "c_student_middlename",
                table: "cd_student",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Отчество студента",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "Отчество студента");

            migrationBuilder.AlterColumn<string>(
                name: "c_student_lastname",
                table: "cd_student",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Фамилия студента",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "Фамилия студента");

            migrationBuilder.AlterColumn<string>(
                name: "c_student_firstname",
                table: "cd_student",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Имя студента",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "Имя студента");

            migrationBuilder.AddColumn<bool>(
                name: "b_is_deleted",
                table: "cd_student",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Признак удаления студента");

            migrationBuilder.AlterColumn<string>(
                name: "c_group_name",
                table: "cd_group",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Название группы",
                oldClrType: typeof(string),
                oldType: "varchar(100)",
                oldMaxLength: 100,
                oldComment: "Название группы");

            migrationBuilder.AddColumn<bool>(
                name: "b_is_deleted",
                table: "cd_group",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Признак удаления группы");

            migrationBuilder.AddColumn<string>(
                name: "c_group_specialty",
                table: "cd_group",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                comment: "Специальность группы");

            migrationBuilder.AddColumn<int>(
                name: "n_group_year",
                table: "cd_group",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Год набора группы");

            migrationBuilder.CreateTable(
                name: "cd_discipline",
                columns: table => new
                {
                    discipline_id = table.Column<int>(type: "int", nullable: false, comment: "Идентификатор записи дисциплины")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    c_discipline_name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "Название дисциплины"),
                    n_discipline_direction = table.Column<int>(type: "int", nullable: false, comment: "Направление дисциплины (1 - гуманитарное, 2 - техническое)"),
                    b_is_deleted = table.Column<bool>(type: "bit", nullable: false, comment: "Признак удаления дисциплины")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cd_discipline_discipline_id", x => x.discipline_id);
                });

            migrationBuilder.CreateTable(
                name: "cd_credit",
                columns: table => new
                {
                    credit_id = table.Column<int>(type: "int", nullable: false, comment: "Идентификатор записи зачета")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    b_credit_is_passed = table.Column<bool>(type: "bit", nullable: false, comment: "Зачет сдан (true) / не сдан (false)"),
                    d_credit_date = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Дата сдачи зачета"),
                    f_student_id = table.Column<int>(type: "int", nullable: false, comment: "Идентификатор студента"),
                    f_discipline_id = table.Column<int>(type: "int", nullable: false, comment: "Идентификатор дисциплины")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cd_credit_credit_id", x => x.credit_id);
                    table.ForeignKey(
                        name: "fk_credit_f_discipline_id",
                        column: x => x.f_discipline_id,
                        principalTable: "cd_discipline",
                        principalColumn: "discipline_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_credit_f_student_id",
                        column: x => x.f_student_id,
                        principalTable: "cd_student",
                        principalColumn: "student_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cd_grade",
                columns: table => new
                {
                    grade_id = table.Column<int>(type: "int", nullable: false, comment: "Идентификатор записи оценки")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    n_grade_value = table.Column<int>(type: "int", nullable: false, comment: "Оценка (от 2 до 5)"),
                    d_grade_date = table.Column<DateTime>(type: "datetime2", nullable: false, comment: "Дата выставления оценки"),
                    f_student_id = table.Column<int>(type: "int", nullable: false, comment: "Идентификатор студента"),
                    f_discipline_id = table.Column<int>(type: "int", nullable: false, comment: "Идентификатор дисциплины")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cd_grade_grade_id", x => x.grade_id);
                    table.ForeignKey(
                        name: "fk_grade_f_discipline_id",
                        column: x => x.f_discipline_id,
                        principalTable: "cd_discipline",
                        principalColumn: "discipline_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_grade_f_student_id",
                        column: x => x.f_student_id,
                        principalTable: "cd_student",
                        principalColumn: "student_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "idx_cd_credit_fk_f_discipline_id",
                table: "cd_credit",
                column: "f_discipline_id");

            migrationBuilder.CreateIndex(
                name: "idx_cd_credit_fk_f_student_id",
                table: "cd_credit",
                column: "f_student_id");

            migrationBuilder.CreateIndex(
                name: "idx_cd_grade_fk_f_discipline_id",
                table: "cd_grade",
                column: "f_discipline_id");

            migrationBuilder.CreateIndex(
                name: "idx_cd_grade_fk_f_student_id",
                table: "cd_grade",
                column: "f_student_id");

            migrationBuilder.AddForeignKey(
                name: "fk_f_group_id",
                table: "cd_student",
                column: "f_group_id",
                principalTable: "cd_group",
                principalColumn: "group_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_f_group_id",
                table: "cd_student");

            migrationBuilder.DropTable(
                name: "cd_credit");

            migrationBuilder.DropTable(
                name: "cd_grade");

            migrationBuilder.DropTable(
                name: "cd_discipline");

            migrationBuilder.DropColumn(
                name: "b_is_deleted",
                table: "cd_student");

            migrationBuilder.DropColumn(
                name: "b_is_deleted",
                table: "cd_group");

            migrationBuilder.DropColumn(
                name: "c_group_specialty",
                table: "cd_group");

            migrationBuilder.DropColumn(
                name: "n_group_year",
                table: "cd_group");

            migrationBuilder.AlterColumn<string>(
                name: "c_student_middlename",
                table: "cd_student",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                comment: "Отчество студента",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Отчество студента");

            migrationBuilder.AlterColumn<string>(
                name: "c_student_lastname",
                table: "cd_student",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Фамилия студента",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "Фамилия студента");

            migrationBuilder.AlterColumn<string>(
                name: "c_student_firstname",
                table: "cd_student",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Имя студента",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "Имя студента");

            migrationBuilder.AlterColumn<string>(
                name: "c_group_name",
                table: "cd_group",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Название группы",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "Название группы");

            migrationBuilder.AddForeignKey(
                name: "fk_f_group_id",
                table: "cd_student",
                column: "f_group_id",
                principalTable: "cd_group",
                principalColumn: "group_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
