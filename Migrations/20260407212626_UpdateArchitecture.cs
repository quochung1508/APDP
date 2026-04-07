using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SIMS.Migrations
{
    /// <inheritdoc />
    public partial class UpdateArchitecture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_class_assignments_Classes_class_id",
                table: "class_assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_class_assignments_Courses_course_id",
                table: "class_assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_class_assignments_students_student_id",
                table: "class_assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_class_assignments_teachers_teacher_id",
                table: "class_assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_Classes_Courses_course_id",
                table: "Classes");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_teachers_TeacherId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_TeacherId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Classes_course_id",
                table: "Classes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_class_assignments",
                table: "class_assignments");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Schedule",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "course_id",
                table: "Classes");

            migrationBuilder.RenameTable(
                name: "class_assignments",
                newName: "ClassAssignments");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Classes",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "teacher_id",
                table: "ClassAssignments",
                newName: "TeacherId");

            migrationBuilder.RenameColumn(
                name: "student_id",
                table: "ClassAssignments",
                newName: "StudentId");

            migrationBuilder.RenameColumn(
                name: "course_id",
                table: "ClassAssignments",
                newName: "CourseId");

            migrationBuilder.RenameColumn(
                name: "class_id",
                table: "ClassAssignments",
                newName: "ClassId");

            migrationBuilder.RenameIndex(
                name: "IX_class_assignments_teacher_id",
                table: "ClassAssignments",
                newName: "IX_ClassAssignments_TeacherId");

            migrationBuilder.RenameIndex(
                name: "IX_class_assignments_student_id",
                table: "ClassAssignments",
                newName: "IX_ClassAssignments_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_class_assignments_course_id",
                table: "ClassAssignments",
                newName: "IX_ClassAssignments_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_class_assignments_class_id",
                table: "ClassAssignments",
                newName: "IX_ClassAssignments_ClassId");

            migrationBuilder.AddColumn<string>(
                name: "Duration",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Major",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cohort",
                table: "Classes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ClassId1",
                table: "ClassAssignments",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Schedule",
                table: "ClassAssignments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassAssignments",
                table: "ClassAssignments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClassAssignments_ClassId1",
                table: "ClassAssignments",
                column: "ClassId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassAssignments_Classes_ClassId",
                table: "ClassAssignments",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassAssignments_Classes_ClassId1",
                table: "ClassAssignments",
                column: "ClassId1",
                principalTable: "Classes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassAssignments_Courses_CourseId",
                table: "ClassAssignments",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassAssignments_students_StudentId",
                table: "ClassAssignments",
                column: "StudentId",
                principalTable: "students",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassAssignments_teachers_TeacherId",
                table: "ClassAssignments",
                column: "TeacherId",
                principalTable: "teachers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassAssignments_Classes_ClassId",
                table: "ClassAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassAssignments_Classes_ClassId1",
                table: "ClassAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassAssignments_Courses_CourseId",
                table: "ClassAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassAssignments_students_StudentId",
                table: "ClassAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassAssignments_teachers_TeacherId",
                table: "ClassAssignments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassAssignments",
                table: "ClassAssignments");

            migrationBuilder.DropIndex(
                name: "IX_ClassAssignments_ClassId1",
                table: "ClassAssignments");

            migrationBuilder.DropColumn(
                name: "Duration",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Major",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "Cohort",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ClassId1",
                table: "ClassAssignments");

            migrationBuilder.DropColumn(
                name: "Schedule",
                table: "ClassAssignments");

            migrationBuilder.RenameTable(
                name: "ClassAssignments",
                newName: "class_assignments");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Classes",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TeacherId",
                table: "class_assignments",
                newName: "teacher_id");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "class_assignments",
                newName: "student_id");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "class_assignments",
                newName: "course_id");

            migrationBuilder.RenameColumn(
                name: "ClassId",
                table: "class_assignments",
                newName: "class_id");

            migrationBuilder.RenameIndex(
                name: "IX_ClassAssignments_TeacherId",
                table: "class_assignments",
                newName: "IX_class_assignments_teacher_id");

            migrationBuilder.RenameIndex(
                name: "IX_ClassAssignments_StudentId",
                table: "class_assignments",
                newName: "IX_class_assignments_student_id");

            migrationBuilder.RenameIndex(
                name: "IX_ClassAssignments_CourseId",
                table: "class_assignments",
                newName: "IX_class_assignments_course_id");

            migrationBuilder.RenameIndex(
                name: "IX_ClassAssignments_ClassId",
                table: "class_assignments",
                newName: "IX_class_assignments_class_id");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "Courses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "Courses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "TeacherId",
                table: "Courses",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Schedule",
                table: "Classes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "course_id",
                table: "Classes",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_class_assignments",
                table: "class_assignments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_TeacherId",
                table: "Courses",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Classes_course_id",
                table: "Classes",
                column: "course_id");

            migrationBuilder.AddForeignKey(
                name: "FK_class_assignments_Classes_class_id",
                table: "class_assignments",
                column: "class_id",
                principalTable: "Classes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_class_assignments_Courses_course_id",
                table: "class_assignments",
                column: "course_id",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_class_assignments_students_student_id",
                table: "class_assignments",
                column: "student_id",
                principalTable: "students",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_class_assignments_teachers_teacher_id",
                table: "class_assignments",
                column: "teacher_id",
                principalTable: "teachers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_Courses_course_id",
                table: "Classes",
                column: "course_id",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_teachers_TeacherId",
                table: "Courses",
                column: "TeacherId",
                principalTable: "teachers",
                principalColumn: "id");
        }
    }
}
