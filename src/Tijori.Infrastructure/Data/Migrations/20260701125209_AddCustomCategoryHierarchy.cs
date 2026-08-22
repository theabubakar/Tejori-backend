using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tijori.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomCategoryHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BucketCategories_Name",
                table: "BucketCategories");

            migrationBuilder.AddColumn<Guid>(
                name: "ParentCategoryId",
                table: "BucketCategories",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BucketCategories_Name",
                table: "BucketCategories",
                column: "Name",
                unique: true,
                filter: "[IsDraft] = 0 AND [ParentCategoryId] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BucketCategories_Name_CreatedByUserId_ParentCategoryId",
                table: "BucketCategories",
                columns: new[] { "Name", "CreatedByUserId", "ParentCategoryId" },
                unique: true,
                filter: "[IsDraft] = 0 AND [ParentCategoryId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BucketCategories_ParentCategoryId",
                table: "BucketCategories",
                column: "ParentCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_BucketCategories_BucketCategories_ParentCategoryId",
                table: "BucketCategories",
                column: "ParentCategoryId",
                principalTable: "BucketCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            var seedTimestamp = new DateTime(2026, 7, 1, 12, 52, 9, DateTimeKind.Utc);
            var customGroupId = Guid.Parse("a1000000-0000-0000-0000-000000000008");

            migrationBuilder.InsertData(
                table: "BucketCategories",
                columns: new[] { "Id", "Name", "IconKey", "SortOrder", "IsActive", "IsDraft", "CreatedByUserId", "ParentCategoryId", "CreatedAt", "UpdatedAt" },
                values: new object[] { customGroupId, "Custom", "custom", 8, true, false, null, null, seedTimestamp, seedTimestamp });

            migrationBuilder.Sql(
                """
                UPDATE BucketCategories
                SET ParentCategoryId = 'a1000000-0000-0000-0000-000000000008',
                    IconKey = 'custom-sub'
                WHERE CreatedByUserId IS NOT NULL
                  AND (IconKey = 'custom' OR ParentCategoryId IS NULL);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BucketCategories_BucketCategories_ParentCategoryId",
                table: "BucketCategories");

            migrationBuilder.DropIndex(
                name: "IX_BucketCategories_Name",
                table: "BucketCategories");

            migrationBuilder.DropIndex(
                name: "IX_BucketCategories_Name_CreatedByUserId_ParentCategoryId",
                table: "BucketCategories");

            migrationBuilder.DropIndex(
                name: "IX_BucketCategories_ParentCategoryId",
                table: "BucketCategories");

            migrationBuilder.DropColumn(
                name: "ParentCategoryId",
                table: "BucketCategories");

            migrationBuilder.CreateIndex(
                name: "IX_BucketCategories_Name",
                table: "BucketCategories",
                column: "Name",
                unique: true,
                filter: "[IsDraft] = 0");
        }
    }
}
