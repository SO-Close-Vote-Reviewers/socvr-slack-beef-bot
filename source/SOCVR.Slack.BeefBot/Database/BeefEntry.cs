using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SOCVR.Slack.BeefBot.Database
{
    public class BeefEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ReporterUserId { get; set; }

        public int OffendingChatUserId { get; set; }

        public string Explanation { get; set; }

        public DateTimeOffset ReportedOn { get; set; }
        public DateTimeOffset ExpiresOn { get; set; }

        public string ShortExplanation
        {
            get
            {
                //cut off at 50 characters

                if (Explanation.Length < 50)
                {
                    return Explanation;
                }

                return Explanation.Substring(0, 50) + "...";
            }
        }
    }
}
