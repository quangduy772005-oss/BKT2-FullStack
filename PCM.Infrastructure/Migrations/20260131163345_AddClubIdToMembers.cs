using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClubIdToMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClubId",
                table: "Members",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Clubs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Activities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ClubId = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activities_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Registrations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MemberId = table.Column<int>(type: "int", nullable: false),
                    ActivityId = table.Column<int>(type: "int", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Registrations_Activities_ActivityId",
                        column: x => x.ActivityId,
                        principalTable: "Activities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Registrations_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 31, 16, 33, 44, 714, DateTimeKind.Utc).AddTicks(2208));

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 31, 16, 33, 44, 714, DateTimeKind.Utc).AddTicks(2211));

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 31, 16, 33, 44, 714, DateTimeKind.Utc).AddTicks(2212));

            migrationBuilder.UpdateData(
                table: "TransactionCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 31, 16, 33, 44, 714, DateTimeKind.Utc).AddTicks(1968));

            migrationBuilder.UpdateData(
                table: "TransactionCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 31, 16, 33, 44, 714, DateTimeKind.Utc).AddTicks(1973));

            migrationBuilder.UpdateData(
                table: "TransactionCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 31, 16, 33, 44, 714, DateTimeKind.Utc).AddTicks(2004));

            migrationBuilder.UpdateData(
                table: "TransactionCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 31, 16, 33, 44, 714, DateTimeKind.Utc).AddTicks(2005));

            migrationBuilder.CreateIndex(
                name: "IX_Members_ClubId",
                table: "Members",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Activities_ClubId",
                table: "Activities",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_ActivityId",
                table: "Registrations",
                column: "ActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_MemberId",
                table: "Registrations",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Clubs_ClubId",
                table: "Members",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_Clubs_ClubId",
                table: "Members");

            migrationBuilder.DropTable(
                name: "Registrations");

            migrationBuilder.DropTable(
                name: "Activities");

            migrationBuilder.DropTable(
                name: "Clubs");

            migrationBuilder.DropIndex(
                name: "IX_Members_ClubId",
                table: "Members");

            migrationBuilder.DropColumn(
                name: "ClubId",
                table: "Members");

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 30, 4, 40, 44, 671, DateTimeKind.Utc).AddTicks(9006));

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 30, 4, 40, 44, 671, DateTimeKind.Utc).AddTicks(9007));

            migrationBuilder.UpdateData(
                table: "Courts",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 30, 4, 40, 44, 671, DateTimeKind.Utc).AddTicks(9009));

            migrationBuilder.UpdateData(
                table: "TransactionCategories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 30, 4, 40, 44, 671, DateTimeKind.Utc).AddTicks(8815));

            migrationBuilder.UpdateData(
                table: "TransactionCategories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 30, 4, 40, 44, 671, DateTimeKind.Utc).AddTicks(8818));

            migrationBuilder.UpdateData(
                table: "TransactionCategories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 30, 4, 40, 44, 671, DateTimeKind.Utc).AddTicks(8820));

            migrationBuilder.UpdateData(
                table: "TransactionCategories",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedDate",
                value: new DateTime(2026, 1, 30, 4, 40, 44, 671, DateTimeKind.Utc).AddTicks(8821));
        }
    }
}
