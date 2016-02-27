using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace SOCVR.Slack.BeefBot.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BeefEntry",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:Serial", true),
                    ExpiresOn = table.Column<DateTimeOffset>(nullable: false),
                    Explanation = table.Column<string>(nullable: true),
                    OffendingChatUserId = table.Column<int>(nullable: false),
                    ReportedOn = table.Column<DateTimeOffset>(nullable: false),
                    ReporterUserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BeefEntry", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("BeefEntry");
        }
    }
}
