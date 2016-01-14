using System;
using System.Linq;
using MargieBot.Models;
using SOCVR.Slack.BeefBot.Database;
using TCL.Extensions;

namespace SOCVR.Slack.BeefBot.Responders
{
    class BeefEventsForResponder : RegexResponder
    {
        public override BotMessage GetResponse(ResponseContext context)
        {
            var offenderUserId = commandPattern.Match(context.Message.Text).Groups[1].Value.Parse<int>();

            using (var db = new DatabaseContext())
            {
                var dbEntries = db.BeefEntries
                    .Where(x => x.OffendingChatUserId == offenderUserId)
                    .OrderBy(x => x.ReportedOn)
                    .ToList();

                var openBeefsCount = dbEntries
                    .Where(x => x.ExpiresOn >= DateTimeOffset.UtcNow)
                    .Count();

                var headerLine = $"There are currently {dbEntries.Count} beefs recorded against {SOChatAccessor.GetUserNameForChatId(offenderUserId)} ({offenderUserId}), {openBeefsCount} of which are active.";

                string resultsTable = "";

                if (dbEntries.Any())
                {
                    resultsTable = dbEntries
                        .ToStringTable(
                            new[]
                            {
                                "Id",
                                "Reported By",
                                "Reported On",
                                "Expires On",
                                "Explanation"
                            },
                            x => x.Id,
                            x => x.ReporterUserId,
                            x => x.ReportedOn.ToString("yyyy-MM-dd HH:mm 'UTC'"),
                            x => x.ExpiresOn.ToString("yyyy-MM-dd HH:mm 'UTC'"),
                            x => x.ShortExplanation);

                    resultsTable = $"```{resultsTable}```";
                }

                var outputMessage = $"{headerLine}\n{resultsTable}";

                return new BotMessage
                {
                    Text = outputMessage
                };
            }
        }

        protected override string GetCommandRegexPattern()
        {
            return @"(?i)^beef events for (\d+)$";
        }
    }
}
