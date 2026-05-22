using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorBlog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHomeAboutSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HomeAboutDescription",
                table: "SiteSettings",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "Modern blog platform built with .NET 10 and Blazor Server. Demonstrates best practices in software development, Clean Architecture and modern technologies.");

            migrationBuilder.AddColumn<string>(
                name: "HomeAboutLinkText",
                table: "SiteSettings",
                type: "character varying(60)",
                maxLength: 60,
                nullable: false,
                defaultValue: "View on GitHub");

            migrationBuilder.AddColumn<string>(
                name: "HomeAboutLinkUrl",
                table: "SiteSettings",
                type: "character varying(300)",
                maxLength: 300,
                nullable: false,
                defaultValue: "https://github.com/unrealbg/BlazorBlog");

            migrationBuilder.AddColumn<string>(
                name: "HomeAboutTitle",
                table: "SiteSettings",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "About This Project");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HomeAboutDescription",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "HomeAboutLinkText",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "HomeAboutLinkUrl",
                table: "SiteSettings");

            migrationBuilder.DropColumn(
                name: "HomeAboutTitle",
                table: "SiteSettings");
        }
    }
}
