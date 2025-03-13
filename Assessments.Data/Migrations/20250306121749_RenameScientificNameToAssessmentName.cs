using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assessments.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameScientificNameToAssessmentName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScientificName",
                table: "Feedbacks");

            migrationBuilder.AddColumn<string>(
                name: "AssessmentName",
                table: "Feedbacks",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssessmentName",
                table: "Feedbacks");

            migrationBuilder.AddColumn<string>(
                name: "ScientificName",
                table: "Feedbacks",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
