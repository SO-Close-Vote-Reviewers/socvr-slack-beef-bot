﻿using MargieBot.Responders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MargieBot.Models;
using SOCVR.Slack.BeefBot.Database;

namespace SOCVR.Slack.BeefBot.Responders
{
    /// <summary>
    /// Shows the beefs that are currently active.
    /// </summary>
    class CurrentBeefsResponder : IResponder
    {


        public bool CanRespond(ResponseContext context)
        {
            return
                context.Message.Text.ToLower() == "beef current" &&
                !context.Message.User.IsSlackbot && // Message must be said by a non-bot.
                context.Message.MentionsBot; // Message must mention the bot.
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            using (var db = new DatabaseContext())
            {
                var currentBeefEntries = db.BeefEntries
                    .Where(x => x.ExpiresOn > DateTimeOffset.UtcNow)
                    .OrderBy(x => x.ReportedOn)
                    .ToList();

                var resultsTable = currentBeefEntries
                    .ToStringTable(
                        new[]
                        {
                            "Offending Member",
                            "Reported By",
                            "Reported On",
                            "Expires On"
                        },
                        x => x.OffendingChatUserId,
                        x => x.ReporterUserId,
                        x => x.ReportedOn.ToString("yyyy-MM-dd HH:mm:ss 'UTC'"),
                        x => x.ExpiresOn.ToString("yyyy-MM-dd HH:mm:ss 'UTC'"));

                var outputMessage = $"Current beefs:{Environment.NewLine}```{resultsTable}```";

                return new BotMessage()
                {
                    Text = outputMessage
                };
            }
        }
    }
}
