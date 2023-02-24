using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Personal_Collection_Manager.Migrations
{
    public partial class LikeIndexCorrected : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Likes_UserId_ItemId_ThumbUp",
                table: "Likes");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId_ItemId",
                table: "Likes",
                columns: new[] { "UserId", "ItemId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Likes_UserId_ItemId",
                table: "Likes");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId_ItemId_ThumbUp",
                table: "Likes",
                columns: new[] { "UserId", "ItemId", "ThumbUp" },
                unique: true);
        }
    }
}
