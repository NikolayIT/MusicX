namespace MusicX.Data.Migrations
{
    using System;

    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class ArtistMetadataAndSongSearchTerms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Artists");

            migrationBuilder.AddColumn<string>(
                name: "SearchTerms",
                table: "Songs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ArtistMetadata",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    ModifiedOn = table.Column<DateTime>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    DeletedOn = table.Column<DateTime>(nullable: true),
                    ArtistId = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: true),
                    SourceId = table.Column<int>(nullable: true),
                    SourceItemId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArtistMetadata", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArtistMetadata_Artists_ArtistId",
                        column: x => x.ArtistId,
                        principalTable: "Artists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ArtistMetadata_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArtistMetadata_ArtistId",
                table: "ArtistMetadata",
                column: "ArtistId");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistMetadata_IsDeleted",
                table: "ArtistMetadata",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_ArtistMetadata_SourceId",
                table: "ArtistMetadata",
                column: "SourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArtistMetadata");

            migrationBuilder.DropColumn(
                name: "SearchTerms",
                table: "Songs");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Artists",
                nullable: true);
        }
    }
}
