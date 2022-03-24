using Microsoft.EntityFrameworkCore.Migrations;

namespace GraphColoring.Infrastructure.Migrations
{
    public partial class initmigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Graphs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdjacencyMatrix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Graphs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlgorithmResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ColoredNodes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfColors = table.Column<int>(type: "int", nullable: false),
                    TimeInMiliseconds = table.Column<long>(type: "bigint", nullable: false),
                    JsonInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GraphId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlgorithmResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlgorithmResults_Graphs_GraphId",
                        column: x => x.GraphId,
                        principalTable: "Graphs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlgorithmResults_GraphId",
                table: "AlgorithmResults",
                column: "GraphId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlgorithmResults");

            migrationBuilder.DropTable(
                name: "Graphs");
        }
    }
}
