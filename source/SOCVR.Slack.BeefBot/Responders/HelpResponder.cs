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
    class HelpResponder : IResponder
    {
        Regex commandPattern = new Regex(@"(?i)^beef help$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

        public bool CanRespond(ResponseContext context)
        {
            return
                commandPattern.IsMatch(context.Message.Text) && //  Must match command regex.
                !context.Message.User.IsSlackbot && // Message must be said by a non-bot.
                context.Message.MentionsBot; // Message must mention the bot.
        }

        public BotMessage GetResponse(ResponseContext context)
        {
            var outputMessage = @"This bot is used for recording disagreements between ROs and chat members, or other unusual disruptions by chat members.

Command list: (`<>` are mandatory, `[]` are optional)

`beef <time frame> <user id> [explanation]` - Records a new beef against a user. `time frame` can be a preset ('low', 'medium', or 'high'), 'X hours', or 'X days'. `Explanation` is optional.
`beef current` - Shows a list of active beefs.
`beef events last <number> days` - Shows a list of all beefs reported in the timeframe given.
`beef events for <user id>` - Shows a list of all beefs reported against the given user.
`beef details <beef number> [beef number] [beef number] ...` - Shows details for the given beef numbers. Provide one or more numbers, space separated.
`beef close <beef number> [beef number] [beef number] ...` - Closes (sets the expiration date to current) the given beefs.
`beef close all for <user id>` - Closes all active beefs for the given user.
`beef help` - Shows this message.";

            return new BotMessage
            {
                Text = outputMessage
            };
        }
    }
}
