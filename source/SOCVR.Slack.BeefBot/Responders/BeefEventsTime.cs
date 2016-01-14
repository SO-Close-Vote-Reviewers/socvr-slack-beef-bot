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
    class BeefEventsTime : IResponder
    {
        Regex commandPattern = new Regex(@"(?i)^beef events last (\d+) days$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public bool CanRespond(ResponseContext context)
        {
            return
                commandPattern.IsMatch(context.Message.Text) && //  Must match command regex.
                !context.Message.User.IsSlackbot && // Message must be said by a non-bot.
                context.Message.MentionsBot; // Message must mention the bot.
        }

        public BotMessage GetResponse(ResponseContext context)
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
    }
}
