using System.ComponentModel.DataAnnotations;

namespace AutomationTennis.Domain
{
    public class TournamentWTA
    {
        [Key]
        public int IdTournamentWTA { get; set; }

        public string Description { get; set; } = string.Empty;

        public string StartDate { get; set; } = string.Empty;

        public string EndDate { get; set; } = string.Empty;

        public string Surface { get; set; } = string.Empty;

        public string InOutdoor {  get; set; } = string.Empty;


        public int SinglesDrawSize { get; set; }

        public int DoublesDrawSize { get; set; }

        public decimal PrizeMoney { get; set; }

        public string PrizeMoneyCurrency { get; set; } = string.Empty;

        public TournamentGroupWTA? TournamentGroupWTA { get; set; }

    }
}
