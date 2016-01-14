﻿using MargieBot.Models;
using MargieBot.Responders;
using SOCVR.Slack.BeefBot.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCL.Extensions;

namespace SOCVR.Slack.BeefBot.Responders
{
    class BeefCloseIndividual : IResponder
    {
        Regex commandPattern = new Regex(@"(?i)^beef close (\d ?)+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public bool CanRespond(ResponseContext context)
        {
            return
                commandPattern.IsMatch(context.Message.Text) && //  Must match command regex.
                !context.Message.User.IsSlackbot && // Message must be said by a non-bot.
                context.Message.MentionsBot; // Message must mention the bot.
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            var beefIds = commandPattern.Match(context.Message.Text)
                 .Groups[1]
                 .Captures
                 .OfType<Capture>()
                 .Select(x => x.Value.Parse<int>())
                 .ToList();

            using (var db = new DatabaseContext())
            {
                var dbEntries = (from dbRecord in db.BeefEntries
                                 join beefId in beefIds on dbRecord.Id equals beefId
                                 select dbRecord)
                                 .OrderBy(x => x.ReportedOn)
                                 .ToList();

                var outputMessage = "";

                if (dbEntries.Count != beefIds.Count)
                {
                    outputMessage = "*Warning, could not locate all requested entries. Ensure you have typed the Id numbers correctly.*\n\n";
                }

                foreach (var dbEntry in dbEntries)
                {
                    if (dbEntry.ExpiresOn < DateTimeOffset.UtcNow)
                    {
                        //the beef has already expired
                        outputMessage += $"Beef #{dbEntry.Id} already expired on {dbEntry.ExpiresOn.ToStandardDisplayString()}.\n";
                    }
                    else
                    {
                        outputMessage += $"Beef #{dbEntry.Id} closed.\n";
                        dbEntry.ExpiresOn = DateTimeOffset.UtcNow;
                    }
                }

                db.SaveChanges();

                return new BotMessage
                {
                    Text = outputMessage
                };
            }
        }
    }
}
