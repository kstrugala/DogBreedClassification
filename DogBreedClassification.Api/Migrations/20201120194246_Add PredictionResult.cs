using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DogBreedClassification.Api.Migrations
{
    public partial class AddPredictionResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PredictionResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Filename = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Image = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PredictedLabel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Probability = table.Column<float>(type: "real", nullable: false),
                    PredictionExecutionTime = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PredictionResults", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PredictionResults");
        }
    }
}
