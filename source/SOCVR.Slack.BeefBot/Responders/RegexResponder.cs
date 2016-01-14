using MargieBot.Responders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MargieBot.Models;
using System.Text.RegularExpressions;

namespace SOCVR.Slack.BeefBot.Responders
{
    abstract class RegexResponder : IResponder
    {
        protected Regex commandPattern;

        public RegexResponder()
        {
            commandPattern = new Regex(GetCommandRegexPattern(), RegexOptions.Compiled | RegexOptions.CultureInvariant);
        }

        public virtual bool CanRespond(ResponseContext context)
        {
            return
                commandPattern.IsMatch(context.Message.Text) && //  Must match command regex.
                !context.Message.User.IsSlackbot && // Message must be said by a non-bot.
                context.Message.MentionsBot; // Message must mention the bot.
        }

        public abstract BotMessage GetResponse(ResponseContext context);

        protected abstract string GetCommandRegexPattern();
    }
}
