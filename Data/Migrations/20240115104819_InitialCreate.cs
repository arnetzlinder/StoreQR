using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreQR.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ClothingItem",
                table: "ClothingItem");

            migrationBuilder.DropColumn(
                name: "ClothingImage",
                table: "ClothingItem");

            migrationBuilder.DropColumn(
                name: "ClothingUser",
                table: "ClothingItem");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ClothingItem");

            migrationBuilder.RenameTable(
                name: "ClothingItem",
                newName: "ClothingItem");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClothingItem",
                table: "ClothingItem",
                column: "ClothingId");

            migrationBuilder.CreateTable(
                name: "FamilyMember",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamilyMember", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StoringUnit",
                columns: table => new
                {
                    StorageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StorageName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StorageDescription = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoringUnit", x => x.StorageId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FamilyMember");

            migrationBuilder.DropTable(
                name: "StoringUnit");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClothingItem",
                table: "ClothingItem");

            migrationBuilder.RenameTable(
                name: "ClothingItem",
                newName: "ClothingItem");

            migrationBuilder.AddColumn<byte[]>(
                name: "ClothingImage",
                table: "ClothingItem",
                type: "varbinary(MAX)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<string>(
                name: "ClothingUser",
                table: "ClothingItem",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ClothingItem",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClothingItem",
                table: "ClothingItem",
                column: "ClothingId");
        }
    }
}
