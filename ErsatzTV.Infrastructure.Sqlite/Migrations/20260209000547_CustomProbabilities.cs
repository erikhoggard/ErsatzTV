using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ErsatzTV.Infrastructure.Sqlite.Migrations
{
    /// <inheritdoc />
    public partial class CustomProbabilities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UseCustomProbabilities",
                table: "ProgramSchedule",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ProgramScheduleLoadDistribution",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProgramScheduleId = table.Column<int>(type: "INTEGER", nullable: false),
                    MediaItemId = table.Column<int>(type: "INTEGER", nullable: true),
                    CollectionId = table.Column<int>(type: "INTEGER", nullable: true),
                    MultiCollectionId = table.Column<int>(type: "INTEGER", nullable: true),
                    SmartCollectionId = table.Column<int>(type: "INTEGER", nullable: true),
                    Weight = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgramScheduleLoadDistribution", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgramScheduleLoadDistribution_Collection_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgramScheduleLoadDistribution_MediaItem_MediaItemId",
                        column: x => x.MediaItemId,
                        principalTable: "MediaItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgramScheduleLoadDistribution_MultiCollection_MultiCollectionId",
                        column: x => x.MultiCollectionId,
                        principalTable: "MultiCollection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgramScheduleLoadDistribution_ProgramSchedule_ProgramScheduleId",
                        column: x => x.ProgramScheduleId,
                        principalTable: "ProgramSchedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgramScheduleLoadDistribution_SmartCollection_SmartCollectionId",
                        column: x => x.SmartCollectionId,
                        principalTable: "SmartCollection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgramScheduleLoadDistribution_CollectionId",
                table: "ProgramScheduleLoadDistribution",
                column: "CollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramScheduleLoadDistribution_MediaItemId",
                table: "ProgramScheduleLoadDistribution",
                column: "MediaItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramScheduleLoadDistribution_MultiCollectionId",
                table: "ProgramScheduleLoadDistribution",
                column: "MultiCollectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramScheduleLoadDistribution_ProgramScheduleId",
                table: "ProgramScheduleLoadDistribution",
                column: "ProgramScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgramScheduleLoadDistribution_SmartCollectionId",
                table: "ProgramScheduleLoadDistribution",
                column: "SmartCollectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgramScheduleLoadDistribution");

            migrationBuilder.DropColumn(
                name: "UseCustomProbabilities",
                table: "ProgramSchedule");
        }
    }
}
