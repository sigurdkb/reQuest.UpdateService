using System;
using System.Collections.Specialized;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl;
using Quartz.Logging;

namespace reQuest.UpdateService
{
    public class Program
    {
        public static IConfigurationRoot Configuration { get; set; }
        public static void Main(string[] args)
        {
            LogProvider.SetCurrentLogProvider(new ConsoleLogProvider());
            ILog log = LogProvider.GetLogger(typeof (DbUpdater));


            string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (String.IsNullOrWhiteSpace(environmentName))
            {
                environmentName = "Development";
            }

            log.Info($"Current environment is: {environmentName}");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional:false, reloadOnChange:true)
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true);

            Configuration = builder.Build();     

            Run().GetAwaiter().GetResult();       

        }

        private static async Task Run()
        {
            try
            {
                // Grab the Scheduler instance from the Factory
                NameValueCollection props = new NameValueCollection
                {
                    // { "quartz.serializer.type", "json" }
                    {"quartz.scheduler.instanceName",  "reQuest-UpdateService"},
                    {"quartz.jobStore.type", "Quartz.Simpl.RAMJobStore, Quartz"},
                    {"quartz.threadPool.threadCount", "3"}

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
                    .WithIdentity("run-delta", "reQuest")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(int.Parse(Program.Configuration["appSettings:frequency"]))
                        .RepeatForever())
                    .Build();

                // Tell quartz to schedule the job using our trigger
                await scheduler.ScheduleJob(job, trigger);

                // Wait and sleep forever
                while (true)
                {
                    await Task.Delay(TimeSpan.FromMinutes(10));
                }

                // and last shut down the scheduler when you are ready to close your program
                // await scheduler.Shutdown();
            }
            catch (SchedulerException se)
            {
                Console.WriteLine(se);
            }
        }


    }
}
