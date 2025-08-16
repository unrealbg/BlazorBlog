namespace BlazorBlog.Migrations
{
    using System;
    using BlazorBlog.Data;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Migrations;

    #nullable disable

    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250815000000_AddIndexesAndRowVersion")]
    public partial class AddIndexesAndRowVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "BlogPosts",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_Slug",
                table: "BlogPosts",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_IsPublished_CategoryId",
                table: "BlogPosts",
                columns: new[] { "IsPublished", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_IsPublished_PublishedAt",
                table: "BlogPosts",
                columns: new[] { "IsPublished", "PublishedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_ViewCount",
                table: "BlogPosts",
                column: "ViewCount");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_Slug",
                table: "BlogPosts");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_IsPublished_CategoryId",
                table: "BlogPosts");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_IsPublished_PublishedAt",
                table: "BlogPosts");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_ViewCount",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "BlogPosts");
        }
    }
}
