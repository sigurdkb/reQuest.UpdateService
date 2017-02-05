using System;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using Quartz.Logging;
using reQuest.UpdateService.Entities;

namespace reQuest.UpdateService
{
    public class DbUpdater : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            ILog log = LogProvider.GetLogger(typeof (DbUpdater));

            using (var db = new reQuestDbContext())
            {
                var activeQuests = db.Quests.Where(q => q.State == QuestState.Active);
                foreach (var quest in activeQuests)
                {
                    if (quest.Ends <= DateTime.UtcNow)
                    {
                        quest.State = QuestState.TimedOut;
                    }
                }
                var result = await db.SaveChangesAsync(); 
                log.Info($"[ { result } ] reQuests timed out");
            } 
        }
    }
}
