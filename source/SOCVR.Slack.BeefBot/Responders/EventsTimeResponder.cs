using System;
using System.Linq;
using MargieBot.Models;
using SOCVR.Slack.BeefBot.Database;
using TCL.Extensions;

namespace SOCVR.Slack.BeefBot.Responders
{
    class BeefEventsTime : RegexResponder
    {
        public override BotMessage GetResponse(ResponseContext context)
        {
            var daysOffset = commandPattern.Match(context.Message.Text).Groups[1].Value.Parse<int>();
            var minimumDateTime = DateTimeOffset.UtcNow.AddDays(-daysOffset);

            using (var db = new DatabaseContext())
            {
                var dbEntries = db.BeefEntries
                    .Where(x => x.ReportedOn >= minimumDateTime)
                    .OrderBy(x => x.ReportedOn)
                    .ToList();

                var headerLine = $"In the past {daysOffset} days, {dbEntries.Count} beefs have been recorded.";

                string resultsTable = "";

                if (dbEntries.Any())
                {
                    resultsTable = dbEntries
                        .ToStringTable(
                            new[]
                            {
                                "Id",
                                "Offender",
                                "Reported By",
                                "Reported On",
                                "Expires On",
                                "Explanation"
                            },
                            x => x.Id,
                            x => $"{SOChatAccessor.GetUserNameForChatId(x.OffendingChatUserId)} ({x.OffendingChatUserId})",
                            x => x.ReporterUserId,
                            x => x.ReportedOn.ToStandardDisplayString(),
                            x => x.ExpiresOn.ToStandardDisplayString(),
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
            return @"(?i)^beef events last (\d+) days$";
        }
    }
}
