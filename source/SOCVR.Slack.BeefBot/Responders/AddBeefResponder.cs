using System;
using MargieBot.Models;
using System.Text.RegularExpressions;
using SOCVR.Slack.BeefBot.Database;
using TCL.Extensions;

namespace SOCVR.Slack.BeefBot.Responders
{
    class AddBeefResponder : RegexResponder
    {
        public override BotMessage GetResponse(ResponseContext context)
        {
            var match = commandPattern.Match(context.Message.Text);

            using (var db = new DatabaseContext())
            {
                var newBeef = new BeefEntry();
                newBeef.ReportedOn = DateTimeOffset.UtcNow;

                var internalMessageUserId = context.Message.User.ID;
                var reportedByUserName = context.UserNameCache[internalMessageUserId];

                newBeef.ReporterUserId = reportedByUserName;
                newBeef.ExpiresOn = DetermineExpirationDate(match);
                newBeef.OffendingChatUserId = match.Groups[4].Value.Parse<int>();
                newBeef.Explanation = match.Groups[5].Value;

                db.BeefEntries.Add(newBeef);
                db.SaveChanges();
            }

            return new BotMessage
            {
                Text = "Saved beef entry."
            };
        }

        protected override string GetCommandRegexPattern()
        {
            return @"(?i)^beef (low|medium|high|(\d) (hours?|days?)) (\d+)(?: (.+))?$";
        }

        private DateTimeOffset DetermineExpirationDate(Match match)
        {
            int hoursToAdd;

            //the severity is either a pre-set "low medium high", or a value in hours or days.
            //first, check if group 2 is used. if so, that means it's a hours/days value
            if (match.Groups[2].Success)
            {
                var valueToAdd = match.Groups[2].Value.Parse<int>();

                //now check group 3 for hours or days
                if (match.Groups[3].Value.ToLower().StartsWith("hour"))
                {
                    hoursToAdd = valueToAdd;
                }
                else
                {
                    hoursToAdd = valueToAdd * 24;
                }
            }
            else
            {
                //this is a preset value
                switch (match.Groups[1].Value.ToLower())
                {
                    case "low":
                        hoursToAdd = SettingsAccessor.GetSetting<int>("LowExpirationHours");
                        break;
                    case "medium":
                        hoursToAdd = SettingsAccessor.GetSetting<int>("MediumExpirationHours");
                        break;
                    case "high":
                        hoursToAdd = SettingsAccessor.GetSetting<int>("HighExpirationHours");
                        break;
                    default:
                        throw new Exception();
                }
            }

            return DateTimeOffset.UtcNow.AddHours(hoursToAdd);
        }
    }
}
