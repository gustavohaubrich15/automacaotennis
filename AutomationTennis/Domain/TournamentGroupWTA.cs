using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace AutomationTennis.Domain
{
    public class TournamentGroupWTA
    {
        [Key]
        public int IdTournamentGroupWTA { get; set; }
        public int IdTournamentGroupWTAIntegration { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Level {  get; set; } = string.Empty;
        public int IdTournamentWTA { get; set; }

        public TournamentWTA TournamentWTA { get; set; } = null!;
    }
}
