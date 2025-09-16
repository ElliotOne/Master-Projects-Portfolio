using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StartupTeam.Module.PortfolioManagement.data.migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "PortfolioManagement");

            migrationBuilder.CreateTable(
                name: "Portfolios",
                schema: "PortfolioManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portfolios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PortfolioItems",
                schema: "PortfolioManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Technologies = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Skills = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Industry = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Duration = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Link = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    AttachmentUrl = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Tags = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    PortfolioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortfolioItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortfolioItems_Portfolios_PortfolioId",
                        column: x => x.PortfolioId,
                        principalSchema: "PortfolioManagement",
                        principalTable: "Portfolios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioItems_PortfolioId",
                schema: "PortfolioManagement",
                table: "PortfolioItems",
                column: "PortfolioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortfolioItems",
                schema: "PortfolioManagement");

            migrationBuilder.DropTable(
                name: "Portfolios",
                schema: "PortfolioManagement");
        }
    }
}
