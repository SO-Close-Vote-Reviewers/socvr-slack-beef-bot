using MargieBot.Responders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MargieBot.Models;
using System.Text.RegularExpressions;
using SOCVR.Slack.BeefBot.Database;
using TCL.Extensions;

namespace SOCVR.Slack.BeefBot.Responders
{
    class BeefCloseAllForUserResponder : IResponder
    {
        Regex commandPattern = new Regex(@"(?i)^beef close all for (\d+)$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public bool CanRespond(ResponseContext context)
        {
            return
                commandPattern.IsMatch(context.Message.Text) && //  Must match command regex.
                !context.Message.User.IsSlackbot && // Message must be said by a non-bot.
                context.Message.MentionsBot; // Message must mention the bot.
        }

        public BotMessage GetResponse(ResponseContext context)
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
    }
}
