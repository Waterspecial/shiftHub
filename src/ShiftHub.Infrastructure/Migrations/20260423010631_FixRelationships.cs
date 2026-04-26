using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShiftHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Organisations_OrganisationId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_OrgMemberships_Organisations_OrganisationId",
                table: "OrgMemberships");

            migrationBuilder.DropForeignKey(
                name: "FK_PayRates_Organisations_OrganisationId",
                table: "PayRates");

            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Organisations_OrganisationId",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_OrganisationId",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_PayRates_OrganisationId",
                table: "PayRates");

            migrationBuilder.DropIndex(
                name: "IX_OrgMemberships_OrganisationId",
                table: "OrgMemberships");

            migrationBuilder.DropIndex(
                name: "IX_Clients_OrganisationId",
                table: "Clients");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "PayRates");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "OrgMemberships");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                table: "Clients");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_OrgId",
                table: "Shifts",
                column: "OrgId");

            migrationBuilder.CreateIndex(
                name: "IX_PayRates_OrgId",
                table: "PayRates",
                column: "OrgId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgMemberships_OrgId",
                table: "OrgMemberships",
                column: "OrgId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_OrgId",
                table: "Clients",
                column: "OrgId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Organisations_OrgId",
                table: "Clients",
                column: "OrgId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrgMemberships_Organisations_OrgId",
                table: "OrgMemberships",
                column: "OrgId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayRates_Organisations_OrgId",
                table: "PayRates",
                column: "OrgId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Organisations_OrgId",
                table: "Shifts",
                column: "OrgId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Organisations_OrgId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_OrgMemberships_Organisations_OrgId",
                table: "OrgMemberships");

            migrationBuilder.DropForeignKey(
                name: "FK_PayRates_Organisations_OrgId",
                table: "PayRates");

            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Organisations_OrgId",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_OrgId",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_PayRates_OrgId",
                table: "PayRates");

            migrationBuilder.DropIndex(
                name: "IX_OrgMemberships_OrgId",
                table: "OrgMemberships");

            migrationBuilder.DropIndex(
                name: "IX_Clients_OrgId",
                table: "Clients");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                table: "Shifts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                table: "PayRates",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                table: "OrgMemberships",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                table: "Clients",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_OrganisationId",
                table: "Shifts",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_PayRates_OrganisationId",
                table: "PayRates",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgMemberships_OrganisationId",
                table: "OrgMemberships",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_OrganisationId",
                table: "Clients",
                column: "OrganisationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Organisations_OrganisationId",
                table: "Clients",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrgMemberships_Organisations_OrganisationId",
                table: "OrgMemberships",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PayRates_Organisations_OrganisationId",
                table: "PayRates",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Organisations_OrganisationId",
                table: "Shifts",
                column: "OrganisationId",
                principalTable: "Organisations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
