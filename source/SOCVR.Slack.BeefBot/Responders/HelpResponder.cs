using MargieBot.Models;

namespace SOCVR.Slack.BeefBot.Responders
{
    class HelpResponder : RegexResponder
    {
        public override BotMessage GetResponse(ResponseContext context)
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

        protected override string GetCommandRegexPattern()
        {
            return @"(?i)^beef help$";
        }
    }
}
