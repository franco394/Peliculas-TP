using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Peliculas.Migrations
{
    /// <inheritdoc />
    public partial class DefaultAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 10, 18, 44, 26, 619, DateTimeKind.Utc).AddTicks(6491), "$2a$13$sTGs7QMp0NBqK01JVk9dTe.PDNjSJhSVoVg6K4.ef3m8DwRRS1MMy" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 7, 10, 18, 43, 3, 330, DateTimeKind.Utc).AddTicks(9901), "$2a$13$/Af4pF1sFle8ypPyar.4g.7VlHwoFwyOKFp8WHqycI9hz9a6xYHXO" });
        }
    }
}
