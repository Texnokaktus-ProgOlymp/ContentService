using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Texnokaktus.ProgOlymp.ContentService.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContestName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContestStage = table.Column<int>(type: "int", nullable: true),
                    ProblemAlias = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ShortName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OverrideFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OverrideContentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(34)", maxLength: 34, nullable: false),
                    OwnerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepositoryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssetName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BucketName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ObjectKey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProblemId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentItems", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentItems_ContestName_ContestStage_ProblemAlias_ShortName",
                table: "ContentItems",
                columns: new[] { "ContestName", "ContestStage", "ProblemAlias", "ShortName" },
                unique: true,
                filter: "[ContestStage] IS NOT NULL AND [ProblemAlias] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentItems");
        }
    }
}
