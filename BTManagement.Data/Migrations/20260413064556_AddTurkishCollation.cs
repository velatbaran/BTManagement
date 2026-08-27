using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BTManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTurkishCollation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase(
                collation: "Turkish_CI_AI");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase(
                oldCollation: "Turkish_CI_AI");
        }
    }
}
