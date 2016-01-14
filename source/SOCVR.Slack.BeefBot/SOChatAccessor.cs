using CsQuery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.BeefBot
{
    static class SOChatAccessor
    {
        /// <summary>
        /// Returns the display name for a person in Stack Overflow chat given their ID number.
        /// For example, "1043380" would return "gunr2171".
        /// </summary>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public static string GetUserNameForChatId(int chatId)
        {
            var url = $"http://chat.stackoverflow.com/users/{chatId}";
            var doc = CQ.CreateFromUrl(url);

            var username = doc["#content .subheader h1"].Text();
            return username;
        }
    }
}
