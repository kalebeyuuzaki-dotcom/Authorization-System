using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Portifolio.Migrations
{
    /// <inheritdoc />
    public partial class optimize_entity_type : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "AccessFailedCount",
                table: "RecoverPasswordrRequests",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<byte>(
                name: "AccessFailedCount",
                table: "PreRegisterRequests",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AccessFailedCount",
                table: "RecoverPasswordrRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");

            migrationBuilder.AlterColumn<int>(
                name: "AccessFailedCount",
                table: "PreRegisterRequests",
                type: "int",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint");
        }
    }
}
