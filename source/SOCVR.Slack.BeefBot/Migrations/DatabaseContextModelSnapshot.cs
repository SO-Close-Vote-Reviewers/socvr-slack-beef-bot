using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using SOCVR.Slack.BeefBot.Database;

namespace SOCVR.Slack.BeefBot.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348");

            modelBuilder.Entity("SOCVR.Slack.BeefBot.Database.BeefEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("ExpiresOn");

                    b.Property<string>("Explanation");

                    b.Property<int>("OffendingChatUserId");

                    b.Property<DateTimeOffset>("ReportedOn");

                    b.Property<string>("ReporterUserId")
                        .IsRequired();

                    b.HasKey("Id");
                });
        }
    }
}
