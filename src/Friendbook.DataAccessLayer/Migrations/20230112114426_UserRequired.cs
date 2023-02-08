using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;


namespace Friendbook.DataAccessLayer.Migrations;

[ExcludeFromCodeCoverage]
/// <inheritdoc />
public partial class UserRequired : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "CreatedBy",
            table: "People",
            type: "varchar(512)",
            unicode: false,
            maxLength: 512,
            nullable: false,
            defaultValue: "",
            oldClrType: typeof(string),
            oldType: "varchar(512)",
            oldUnicode: false,
            oldMaxLength: 512,
            oldNullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "CreatedBy",
            table: "People",
            type: "varchar(512)",
            unicode: false,
            maxLength: 512,
            nullable: true,
            oldClrType: typeof(string),
            oldType: "varchar(512)",
            oldUnicode: false,
            oldMaxLength: 512);
    }
}
