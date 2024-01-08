using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreQR.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClothingItems",
                columns: table => new
                {
                    ClothingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    ClothingImage = table.Column<byte[]>(type: "varbinary(MAX)", nullable: false),
                    ClothingName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClothingUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClothingBrand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClothingSize = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClothingColor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Season = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClothingMaterial = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TypeOfClothing = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClothingItems", x => x.ClothingId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClothingItems");
        }
    }
}
