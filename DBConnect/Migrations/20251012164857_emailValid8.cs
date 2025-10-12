using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DBConnect.Migrations
{
    /// <inheritdoc />
    public partial class emailValid8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailValidations",
                table: "EmailValidations");

            migrationBuilder.AlterColumn<byte[]>(
                name: "ValCode",
                table: "EmailValidations",
                type: "varbinary(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "EmailValidations",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "EmailValidations",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255)
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailValidations",
                table: "EmailValidations",
                columns: new[] { "CreatedAt", "Email" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EmailValidations",
                table: "EmailValidations");

            migrationBuilder.AlterColumn<int>(
                name: "ValCode",
                table: "EmailValidations",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(32)",
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "EmailValidations",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255)
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "EmailValidations",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmailValidations",
                table: "EmailValidations",
                column: "Email");
        }
    }
}
