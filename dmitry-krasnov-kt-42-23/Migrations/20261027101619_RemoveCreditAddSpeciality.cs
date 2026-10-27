using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace dmitry_krasnov_kt_42_23.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCreditAddSpeciality : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Удаляем таблицу зачетов (вместе с ее внешними ключами и индексами)
            migrationBuilder.DropTable(
                name: "cd_credit");

            // 2. Создаем таблицу специальностей
            migrationBuilder.CreateTable(
                name: "cd_speciality",
                columns: table => new
                {
                    speciality_id = table.Column<int>(type: "int", nullable: false, comment: "Идентификатор записи специальности")
                        .Annotation("SqlServer:Identity", "1, 1"),
                    c_speciality_title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "Название специальности"),
                    c_speciality_code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, comment: "Код специальности"),
                    b_is_deleted = table.Column<bool>(type: "bit", nullable: false, comment: "Признак удаления специальности")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cd_speciality_speciality_id", x => x.speciality_id);
                });

            // 3. Переносим уже существующие специальности из текстового поля групп в новую таблицу
            migrationBuilder.Sql(@"
                INSERT INTO cd_speciality (c_speciality_title, b_is_deleted)
                SELECT DISTINCT c_group_specialty, 0
                FROM cd_group;");

            // 4. Добавляем в группы внешний ключ на специальность
            migrationBuilder.AddColumn<int>(
                name: "f_speciality_id",
                table: "cd_group",
                type: "int",
                nullable: false,
                defaultValue: 0,
                comment: "Идентификатор специальности");

            // 5. Проставляем каждой группе Id ее специальности (по совпадению названия)
            migrationBuilder.Sql(@"
                UPDATE g
                SET g.f_speciality_id = s.speciality_id
                FROM cd_group g
                JOIN cd_speciality s ON s.c_speciality_title = g.c_group_specialty;");

            // 6. Старое текстовое поле больше не нужно
            migrationBuilder.DropColumn(
                name: "c_group_specialty",
                table: "cd_group");

            migrationBuilder.CreateIndex(
                name: "idx_cd_group_fk_f_speciality_id",
                table: "cd_group",
                column: "f_speciality_id");

            migrationBuilder.AddForeignKey(
                name: "fk_f_speciality_id",
                table: "cd_group",
                column: "f_speciality_id",
                principalTable: "cd_speciality",
                principalColumn: "speciality_id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_f_speciality_id",
                table: "cd_group");

            migrationBuilder.DropIndex(
                name: "idx_cd_group_fk_f_speciality_id",
                table: "cd_group");

            // Возвращаем текстовое поле специальности и заполняем его из таблицы специальностей
            migrationBuilder.AddColumn<string>(
                name: "c_group_specialty",
                table: "cd_group",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "",
                comment: "Специальность группы");

            migrationBuilder.Sql(@"
                UPDATE g
                SET g.c_group_specialty = s.c_speciality_title
                FROM cd_group g
                JOIN cd_speciality s ON s.speciality_id = g.f_speciality_id;");

            migrationBuilder.DropColumn(
                name: "f_speciality_id",
                table: "cd_group");

            migrationBuilder.DropTable(
                name: "cd_speciality");

            // Восстанавливаем таблицу зачетов (данные в ней при откате будут пустыми)
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

            migrationBuilder.CreateIndex(
                name: "idx_cd_credit_fk_f_discipline_id",
                table: "cd_credit",
                column: "f_discipline_id");

            migrationBuilder.CreateIndex(
                name: "idx_cd_credit_fk_f_student_id",
                table: "cd_credit",
                column: "f_student_id");
        }
    }
}
