using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations;


namespace Friendbook.DataAccessLayer.Migrations;

[ExcludeFromCodeCoverage]
/// <inheritdoc />
public partial class Initial : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "People",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                FirstName = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                LastName = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                City = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                Photo = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                CreatedBy = table.Column<string>(type: "varchar(512)", unicode: false, maxLength: 512, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_People", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "People");
    }
}
