using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace reQuest.UpdateService.Entities
{
	[Flags]
	public enum QuestState
    {
		None = 0,
		Done = 1,
        Active = 2,
        TimedOut = 4,
		Approved = 8
        
    };

    public class Quest
    {
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
        public QuestState State { get; set; } = QuestState.Active;
        public DateTime Created { get; set; }
        public DateTime Ends { get; set; }
		public ICollection<Player> ActivePlayers { get; set; } = new List<Player>();
		public ICollection<Player> PassivePlayers { get; set; } = new List<Player>();

		[ForeignKey(nameof(TopicId))]
		public Topic Topic { get; set; }
		public string TopicId { get; set; }
		[ForeignKey(nameof(OwnerId))]
		public Player Owner { get; set; }
		public string OwnerId { get; set; }
		[ForeignKey(nameof(WinnerId))]
		public Player Winner { get; set; }
		public string WinnerId { get; set; }

    }
}
