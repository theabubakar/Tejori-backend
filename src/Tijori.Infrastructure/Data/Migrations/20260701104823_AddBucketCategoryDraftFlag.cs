using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tijori.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBucketCategoryDraftFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BucketCategories_Name",
                table: "BucketCategories");

            migrationBuilder.AddColumn<bool>(
                name: "IsDraft",
                table: "BucketCategories",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_BucketCategories_Name",
                table: "BucketCategories",
                column: "Name",
                unique: true,
                filter: "[IsDraft] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BucketCategories_Name",
                table: "BucketCategories");

            migrationBuilder.DropColumn(
                name: "IsDraft",
                table: "BucketCategories");

            migrationBuilder.CreateIndex(
                name: "IX_BucketCategories_Name",
                table: "BucketCategories",
                column: "Name",
                unique: true);
        }
    }
}
