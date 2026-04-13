using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class rfpatch1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectStages_StageStatuses_ProjectId",
                table: "ProjectStages");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStages_StageStatusId",
                table: "ProjectStages",
                column: "StageStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectStages_StageStatuses_StageStatusId",
                table: "ProjectStages",
                column: "StageStatusId",
                principalTable: "StageStatuses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectStages_StageStatuses_StageStatusId",
                table: "ProjectStages");

            migrationBuilder.DropIndex(
                name: "IX_ProjectStages_StageStatusId",
                table: "ProjectStages");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectStages_StageStatuses_ProjectId",
                table: "ProjectStages",
                column: "ProjectId",
                principalTable: "StageStatuses",
                principalColumn: "Id");
        }
    }
}
