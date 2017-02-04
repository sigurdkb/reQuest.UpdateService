using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;
using reQuest.UpdateService.Entities;

namespace reQuest.UpdateService
{
    public class Program
    {
        public static IConfigurationRoot Configuration { get; set; }
        public static void Main(string[] args)
        {
            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());

            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (String.IsNullOrWhiteSpace(environmentName))
            {
                // throw new ArgumentNullException("Environment not found in ASPNETCORE_ENVIRONMENT");
                environmentName = "Development";
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional:false, reloadOnChange:true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true);

            Configuration = builder.Build();     

            Run().GetAwaiter().GetResult();       

            // using (var db = new reQuestDbContext())
            // {
            //     var activeQuests = db.Quests.Where(q => q.State == QuestState.Active);
            //     foreach (var quest in activeQuests)
            //     {
            //         if (quest.Ends <= DateTime.UtcNow)
            //         {
            //             quest.State = QuestState.TimedOut;
            //         }
            //     }
            //     db.SaveChanges(); 
            // } 
        }

        private static async Task Run()
        {
            try
            {
                // Grab the Scheduler instance from the Factory
                NameValueCollection props = new NameValueCollection
                {
                    // { "quartz.serializer.type", "json" }
                };
                StdSchedulerFactory factory = new StdSchedulerFactory(props);
                IScheduler scheduler = await factory.GetScheduler();

                // and start it off
                await scheduler.Start();

                // define the job and tie it to our HelloJob class
                IJobDetail job = JobBuilder.Create<DbUpdater>()
                    .WithIdentity("update-database", "reQuest")
                    .Build();

                // Trigger the job to run now, and then repeat every 10 seconds
                ITrigger trigger = TriggerBuilder.Create()
                    .WithIdentity("run-delta-60s", "reQuest")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(10)
                        .RepeatForever())
                    .Build();

                // Tell quartz to schedule the job using our trigger
                await scheduler.ScheduleJob(job, trigger);

                // some sleep to show what's happening
                await Task.Delay(TimeSpan.FromSeconds(60));

                // and last shut down the scheduler when you are ready to close your program
                await scheduler.Shutdown();
            }
            catch (SchedulerException se)
            {
                Console.WriteLine(se);
            }
        }


    }
}
