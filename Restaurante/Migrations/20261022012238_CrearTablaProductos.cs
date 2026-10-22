using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Restaurante.Migrations
{
    /// <inheritdoc />
    public partial class CrearTablaProductos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    ID_Producto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ID_Categoria = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.ID_Producto);
                    table.ForeignKey(
                        name: "FK_Productos_Categorias_ID_Categoria",
                        column: x => x.ID_Categoria,
                        principalTable: "Categorias",
                        principalColumn: "ID_Categoria",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_ID_Categoria",
                table: "Productos",
                column: "ID_Categoria");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Productos");
        }
    }
}
