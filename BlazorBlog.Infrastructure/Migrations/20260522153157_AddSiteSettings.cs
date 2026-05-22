using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BlazorBlog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    HomeHeroTitle = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    HomeHeroSubtitle = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    HomeHeroTags = table.Column<string>(type: "character varying(180)", maxLength: 180, nullable: false),
                    HomeHeroPrimaryButtonText = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    HomeHeroPrimaryButtonUrl = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    HomeHeroSecondaryButtonText = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    HomeHeroSecondaryButtonUrl = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteSettings");
        }
    }
}
