using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MargieBot;
using SOCVR.Slack.BeefBot.Responders;
using SOCVR.Slack.BeefBot.Database;

namespace SOCVR.Slack.BeefBot
{
    class Program
    {
        static Bot bot = new Bot();
        static ManualResetEvent exitMre = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            var cs = SettingsAccessor.GetSetting<string>("DBConnectionString");
            var botAPIKey = SettingsAccessor.GetSetting<string>("SlackBotAPIKey");

            //inital connection to database
            using (var db = new DatabaseContext())
            {
                db.Database.EnsureCreated();
            }

            bot.Aliases = new List<string>() { "beef" };
            bot.Responders.Add(new AddBeefResponder());
            bot.Responders.Add(new CurrentBeefsResponder());
            bot.Responders.Add(new BeefEventsTime());
            bot.Connect(botAPIKey);

            Console.CancelKeyPress += delegate
            {
                bot.Disconnect();
                Console.WriteLine("Got signal to shut down.");
                exitMre.Set();
            };

            exitMre.WaitOne();
        }
    }
}
