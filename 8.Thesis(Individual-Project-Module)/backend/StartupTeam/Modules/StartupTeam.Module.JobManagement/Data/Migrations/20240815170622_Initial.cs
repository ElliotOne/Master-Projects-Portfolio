using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StartupTeam.Module.JobManagement.data.migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "JobManagement");

            migrationBuilder.CreateTable(
                name: "JobAdvertisements",
                schema: "JobManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartupName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    StartupDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartupStage = table.Column<int>(type: "int", nullable: false),
                    Industry = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    KeyTechnologies = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UniqueSellingPoints = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MissionStatement = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FoundingYear = table.Column<int>(type: "int", nullable: true),
                    TeamSize = table.Column<int>(type: "int", nullable: true),
                    StartupWebsite = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    StartupValues = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobTitle = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    JobDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmploymentType = table.Column<int>(type: "int", nullable: false),
                    RequiredSkills = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JobResponsibilities = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalaryRange = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    JobLocationType = table.Column<int>(type: "int", nullable: false),
                    JobLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationDeadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Experience = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Education = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequireCV = table.Column<bool>(type: "bit", nullable: false),
                    RequireCoverLetter = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobAdvertisements", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobAdvertisements",
                schema: "JobManagement");
        }
    }
}
