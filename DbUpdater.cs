using System;
using System.Linq;
using System.Threading.Tasks;
using Quartz;
using reQuest.UpdateService.Entities;

namespace reQuest.UpdateService
{
    public class DbUpdater : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            using (var db = new reQuestDbContext())
            {
                var activeQuests = db.Quests.Where(q => q.State == QuestState.Active);
                foreach (var quest in activeQuests)
                {
                    if (quest.Ends <= DateTime.UtcNow)
                    {
                        quest.State = QuestState.TimedOut;
                        await Console.Error.WriteLineAsync();
                    }
                }
                var result = await db.SaveChangesAsync(); 
                await Console.Error.WriteLineAsync($"[ { result } ] reQuests timed out");
            } 
        }
    }
}
