using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.BeefBot
{
    public static class Extensions
    {
        public static string ToStandardDisplayString(this DateTimeOffset datetime)
        {
            return datetime.ToString("yyyy-MM-dd HH:mm 'UTC'");
        }
    }
}
