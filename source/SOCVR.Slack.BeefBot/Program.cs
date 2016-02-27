using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MargieBot;
using SOCVR.Slack.BeefBot.Responders;
using SOCVR.Slack.BeefBot.Database;
using Microsoft.Data.Entity;
using System.Net.Sockets;

namespace SOCVR.Slack.BeefBot
{
    class Program
    {
        static Bot bot = new Bot();
        static ManualResetEvent exitMre = new ManualResetEvent(false);

        static void Main(string[] args)
        {
            Console.WriteLine("Starting program");

            var cs = SettingsAccessor.GetSetting<string>("DBConnectionString");
            var botAPIKey = SettingsAccessor.GetSetting<string>("SlackBotAPIKey");

            InitializeDatabase();

            bot.Aliases = new List<string>() { "beef" };
            bot.Responders.Add(new AddBeefResponder());
            bot.Responders.Add(new CurrentBeefsResponder());
            bot.Responders.Add(new BeefEventsTime());
            bot.Responders.Add(new BeefEventsForResponder());
            bot.Responders.Add(new BeefDetailsResponder());
            bot.Responders.Add(new BeefCloseIndividual());
            bot.Responders.Add(new BeefCloseAllForUserResponder());
            bot.Responders.Add(new HelpResponder());

            Console.WriteLine("Starting slack connection");
            var botConnection = bot.Connect(botAPIKey);

            bot.ConnectionStatusChanged += Bot_ConnectionStatusChanged;

            Console.CancelKeyPress += delegate
            {
                bot.Disconnect();
                Console.WriteLine("Got signal to shut down.");
                exitMre.Set();
            };

            exitMre.WaitOne();
        }

        private static void Bot_ConnectionStatusChanged(bool isConnected)
        {
            if (!isConnected)
            {
                //if you got disconnected, exit
                Console.WriteLine("Got disconnected from slack, exiting");
                exitMre.Set();
            }
        }

        private static void InitializeDatabase()
        {
            Console.WriteLine("Setting up database.");

            //initial connection to database
            using (var db = new DatabaseContext())
            {
                bool dbSetUp = false;

                //loop until the connection works
                while (!dbSetUp)
                {
                    try
                    {
                        //create the database if it does not exist and push and new migrations to it
                        db.Database.Migrate();
                        dbSetUp = true;
                    }
                    catch (SocketException ex)
                    {
                        Console.WriteLine("Caught error when trying to set up database. Waiting 30 seconds to retry.");
                        Console.WriteLine(ex.Message);
                        Thread.Sleep(30 * 1000);
                    }
                }
            }
        }
    }
}
