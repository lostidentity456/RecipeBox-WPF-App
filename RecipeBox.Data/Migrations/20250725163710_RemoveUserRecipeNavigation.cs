using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeBox.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserRecipeNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recipes_Users_AuthorId",
                table: "Recipes");

            migrationBuilder.DropIndex(
                name: "IX_Recipes_AuthorId",
                table: "Recipes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Recipes_AuthorId",
                table: "Recipes",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recipes_Users_AuthorId",
                table: "Recipes",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
