using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class rfpatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Collaborations_CollaboratorTypes_CollaboratorTypeId",
                table: "Collaborations");

            migrationBuilder.DropForeignKey(
                name: "FK_Collaborations_RequestStatuses_RequestStatusId",
                table: "Collaborations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectStages_StageStatuses_StageStatusId",
                table: "ProjectStages");

            migrationBuilder.DropForeignKey(
                name: "FK_Supports_SupportTypes_SupportTypeId",
                table: "Supports");

            migrationBuilder.DropIndex(
                name: "IX_ProjectStages_StageStatusId",
                table: "ProjectStages");

            migrationBuilder.AddForeignKey(
                name: "FK_Collaborations_CollaboratorTypes_CollaboratorTypeId",
                table: "Collaborations",
                column: "CollaboratorTypeId",
                principalTable: "CollaboratorTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Collaborations_RequestStatuses_RequestStatusId",
                table: "Collaborations",
                column: "RequestStatusId",
                principalTable: "RequestStatuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectStages_StageStatuses_ProjectId",
                table: "ProjectStages",
                column: "ProjectId",
                principalTable: "StageStatuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Supports_SupportTypes_SupportTypeId",
                table: "Supports",
                column: "SupportTypeId",
                principalTable: "SupportTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Collaborations_CollaboratorTypes_CollaboratorTypeId",
                table: "Collaborations");

            migrationBuilder.DropForeignKey(
                name: "FK_Collaborations_RequestStatuses_RequestStatusId",
                table: "Collaborations");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectStages_StageStatuses_ProjectId",
                table: "ProjectStages");

            migrationBuilder.DropForeignKey(
                name: "FK_Supports_SupportTypes_SupportTypeId",
                table: "Supports");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStages_StageStatusId",
                table: "ProjectStages",
                column: "StageStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Collaborations_CollaboratorTypes_CollaboratorTypeId",
                table: "Collaborations",
                column: "CollaboratorTypeId",
                principalTable: "CollaboratorTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Collaborations_RequestStatuses_RequestStatusId",
                table: "Collaborations",
                column: "RequestStatusId",
                principalTable: "RequestStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectStages_StageStatuses_StageStatusId",
                table: "ProjectStages",
                column: "StageStatusId",
                principalTable: "StageStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Supports_SupportTypes_SupportTypeId",
                table: "Supports",
                column: "SupportTypeId",
                principalTable: "SupportTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
