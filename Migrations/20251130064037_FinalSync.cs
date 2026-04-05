using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SIMS.Migrations
{
    /// <inheritdoc />
    public partial class FinalSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "aspnetroles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnetroles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "aspnetusers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnetusers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "students",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    StudentNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "teachers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    StaffNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Department = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_teachers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "aspnetroleclaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnetroleclaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_aspnetroleclaims_aspnetroles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "aspnetroles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspnetuserclaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnetuserclaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_aspnetuserclaims_aspnetusers_UserId",
                        column: x => x.UserId,
                        principalTable: "aspnetusers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspnetuserlogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnetuserlogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_aspnetuserlogins_aspnetusers_UserId",
                        column: x => x.UserId,
                        principalTable: "aspnetusers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspnetuserroles",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnetuserroles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_aspnetuserroles_aspnetroles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "aspnetroles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_aspnetuserroles_aspnetusers_UserId",
                        column: x => x.UserId,
                        principalTable: "aspnetusers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "aspnetusertokens",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_aspnetusertokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_aspnetusertokens_aspnetusers_UserId",
                        column: x => x.UserId,
                        principalTable: "aspnetusers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "aspnetroles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1L, null, "Admin", "ADMIN" },
                    { 2L, null, "Teacher", "TEACHER" },
                    { 3L, null, "Student", "STUDENT" },
                    { 4L, null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "aspnetusers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { 2L, 0, "1f20f25a-baf3-459e-a596-9c161caaa33c", "t1@mail.com", true, false, null, "T1@MAIL.COM", "TEACHER1", "AQAAAAIAAYagAAAAEAkcR5/UfOiMP3Zt5FIu+C2qfhPnJinFDx9h50baXnEq2yDMmaxI4y2FkekJIErocQ==", null, false, "db5d9651-83f8-4c05-bcd7-87bb4ed261be", false, "teacher1" },
                    { 5L, 0, "f8291f5d-ded7-416e-9c53-445c0557735e", "s1@mail.com", true, false, null, "S1@MAIL.COM", "STUDENT1", "AQAAAAIAAYagAAAAEAkcR5/UfOiMP3Zt5FIu+C2qfhPnJinFDx9h50baXnEq2yDMmaxI4y2FkekJIErocQ==", null, false, "3e25fb21-b55c-4f0a-b738-15a8ea0a337a", false, "student1" },
                    { 6L, 0, "7737efd8-3a21-4013-afec-3b24a0e908f2", "s2@mail.com", true, false, null, "S2@MAIL.COM", "STUDENT2", "AQAAAAIAAYagAAAAEAkcR5/UfOiMP3Zt5FIu+C2qfhPnJinFDx9h50baXnEq2yDMmaxI4y2FkekJIErocQ==", null, false, "be3f6a2d-96cc-4016-9acd-ec4a2778a36d", false, "student2" },
                    { 9L, 0, "aaa62926-e652-474b-804a-953d232f2b1e", "s5@mail.com", true, false, null, "S5@MAIL.COM", "STUDENT5", "AQAAAAIAAYagAAAAEAkcR5/UfOiMP3Zt5FIu+C2qfhPnJinFDx9h50baXnEq2yDMmaxI4y2FkekJIErocQ==", null, false, "4902137b-704b-42b8-a731-0d7966465886", false, "student5" },
                    { 12L, 0, "bf929322-8635-4901-83ff-c2c3411f0a07", "sysadmin@example.com", true, false, null, "SYSADMIN@EXAMPLE.COM", "SYSADMIN", "AQAAAAIAAYagAAAAEAkcR5/UfOiMP3Zt5FIu+C2qfhPnJinFDx9h50baXnEq2yDMmaxI4y2FkekJIErocQ==", null, false, "91bdeddf-9518-44e0-9a3a-26add81ba47b", false, "sysadmin" },
                    { 16L, 0, "9e92648a-1145-4eae-bd99-140d50252534", "admin@example.com", true, false, null, "ADMIN@EXAMPLE.COM", "ADMIN", "AQAAAAIAAYagAAAAEAkcR5/UfOiMP3Zt5FIu+C2qfhPnJinFDx9h50baXnEq2yDMmaxI4y2FkekJIErocQ==", null, false, "17b0f00a-647d-47ed-975b-f743e4f365d0", false, "admin" },
                    { 17L, 0, "4938aa81-5b3d-4916-941c-6970c8d29bc0", "dotrang2004mc@gmail.com", true, false, null, "DOTRANG2004MC@GMAIL.COM", "ABC", "AQAAAAIAAYagAAAAEAkcR5/UfOiMP3Zt5FIu+C2qfhPnJinFDx9h50baXnEq2yDMmaxI4y2FkekJIErocQ==", null, false, "476c3a2b-7831-45a1-b6b6-73781dce6013", false, "abc" },
                    { 18L, 0, "a1d016fc-4d53-4c80-94b5-0c550f4c1527", "ducd53776@gmail.com", true, false, null, "DUCD53776@GMAIL.COM", "DUCDM", "AQAAAAIAAYagAAAAEAkcR5/UfOiMP3Zt5FIu+C2qfhPnJinFDx9h50baXnEq2yDMmaxI4y2FkekJIErocQ==", null, false, "9503ffc3-8df5-4e24-808d-9c6ba3802000", false, "ducdm" },
                    { 19L, 0, "7f2c7595-871b-44c9-9657-7298d34bcccc", "mu@gamil.com", true, false, null, "MU@GAMIL.COM", "DOAN", "AQAAAAIAAYagAAAAEAkcR5/UfOiMP3Zt5FIu+C2qfhPnJinFDx9h50baXnEq2yDMmaxI4y2FkekJIErocQ==", null, false, "41667a3d-8738-4434-ae0f-6669bfcaa1e0", false, "doan" },
                    { 20L, 0, "f7f06751-9283-4bac-96d8-299cfb7ecdd4", "duc6@gmail.com", true, false, null, "DUC6@GMAIL.COM", "DOANMINHDUC", "AQAAAAIAAYagAAAAEAkcR5/UfOiMP3Zt5FIu+C2qfhPnJinFDx9h50baXnEq2yDMmaxI4y2FkekJIErocQ==", null, false, "47afb4fc-cc99-4770-b720-cd69c441cb57", false, "doanminhduc" }
                });

            migrationBuilder.InsertData(
                table: "aspnetuserroles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { 2L, 2L },
                    { 3L, 5L },
                    { 3L, 6L },
                    { 3L, 9L },
                    { 1L, 12L },
                    { 1L, 16L },
                    { 4L, 17L },
                    { 4L, 18L },
                    { 4L, 19L },
                    { 4L, 20L }
                });

            migrationBuilder.CreateIndex(
                name: "IX_aspnetroleclaims_RoleId",
                table: "aspnetroleclaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "aspnetroles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_aspnetuserclaims_UserId",
                table: "aspnetuserclaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_aspnetuserlogins_UserId",
                table: "aspnetuserlogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_aspnetuserroles_RoleId",
                table: "aspnetuserroles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "aspnetusers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "aspnetusers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "aspnetroleclaims");

            migrationBuilder.DropTable(
                name: "aspnetuserclaims");

            migrationBuilder.DropTable(
                name: "aspnetuserlogins");

            migrationBuilder.DropTable(
                name: "aspnetuserroles");

            migrationBuilder.DropTable(
                name: "aspnetusertokens");

            migrationBuilder.DropTable(
                name: "students");

            migrationBuilder.DropTable(
                name: "teachers");

            migrationBuilder.DropTable(
                name: "aspnetroles");

            migrationBuilder.DropTable(
                name: "aspnetusers");
        }
    }
}
