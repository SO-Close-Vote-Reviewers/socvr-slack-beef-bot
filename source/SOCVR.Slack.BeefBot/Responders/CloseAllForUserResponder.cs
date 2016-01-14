using System;
using System.Linq;
using MargieBot.Models;
using SOCVR.Slack.BeefBot.Database;
using TCL.Extensions;

namespace SOCVR.Slack.BeefBot.Responders
{
    class BeefCloseAllForUserResponder : RegexResponder
    {
        public override BotMessage GetResponse(ResponseContext context)
        {
            var offendingUserId = commandPattern.Match(context.Message.Text).Groups[1].Value.Parse<int>();

            using (var db = new DatabaseContext())
            {
                var dbEntriesForUser = db.BeefEntries
                    .Where(x => x.OffendingChatUserId == offendingUserId)
                    .Where(x => x.ExpiresOn > DateTimeOffset.UtcNow) //only get ones that are active
                    .OrderBy(x => x.ReportedOn)
                    .ToList();

                string outputMessage = "";

                if (!dbEntriesForUser.Any())
                {
                    outputMessage = "This user has no active beefs.";
                }
                else
                {
                    foreach (var dbEntry in dbEntriesForUser)
                    {
                        outputMessage += $"Closing beef #{dbEntry.Id}.\n";
                        dbEntry.ExpiresOn = DateTimeOffset.UtcNow;
                    }

                    db.SaveChanges();
                }

                return new BotMessage
                {
                    Text = outputMessage
                };
            }
        }

        protected override string GetCommandRegexPattern()
        {
            return @"(?i)^beef close all for (\d+)$";
        }
    }
}
