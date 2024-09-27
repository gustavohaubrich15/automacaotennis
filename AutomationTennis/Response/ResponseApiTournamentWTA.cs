using Newtonsoft.Json;

namespace AutomationTennis.Response
{
    [Serializable]
    public class ResponseApiTournamentWTA
    {
        [JsonProperty("content")]
        public List<ResponseApiListTournament> Content { get; set; } = new List<ResponseApiListTournament>();
    }

    [Serializable]
    public class ResponseApiListTournament
    {
        [JsonProperty("tournamentGroup")]
        public ResponseApiTournamentGroup TournamentGroup { get; set; } = new ResponseApiTournamentGroup();

        [JsonProperty("year")]
        public int Year { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; } = string.Empty;

        [JsonProperty("startDate")]
        public string StartDate { get; set; } = string.Empty;

        [JsonProperty("endDate")]
        public string EndDate { get; set; } = string.Empty;

        [JsonProperty("surface")]
        public string Surface { get; set; } = string.Empty;

        [JsonProperty("inOutdoor")]
        public string InOutdoor { get; set; } = string.Empty;

        [JsonProperty("city")]
        public string City { get; set; } = string.Empty;

        [JsonProperty("country")]
        public string Country { get; set; } = string.Empty;

        [JsonProperty("singlesDrawSize")]
        public int SinglesDrawSize { get; set; }

        [JsonProperty("doublesDrawSize")]
        public int DoublesDrawSize { get; set; }

        [JsonProperty("prizeMoney")]
        public decimal PrizeMoney { get; set; }

        [JsonProperty("prizeMoneyCurrency")]
        public string PrizeMoneyCurrency { get; set; } = string.Empty;

        [JsonProperty("liveScoringId")]
        public string LiveScoringId { get; set; } = string.Empty;
    }

    [Serializable]
    public class ResponseApiTournamentGroup
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        [JsonProperty("level")]
        public string Level { get; set; } = string.Empty;

        [JsonProperty("metadata")]
        public object? Metadata { get; set; }
    }
}
