using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace AutomationTennis.Response
{
	[Serializable]
	public class ResponseApiMatchDayWTAFormat2
	{
		[JsonProperty("matches")]
		public List<MatchFormat2> Matches { get; set; } = new List<MatchFormat2>();
	}

	[Serializable]
	public class MatchFormat2
	{
		[JsonPropertyName("CourtName")]
		public string CourtName { get; set; } = string.Empty;

		[JsonPropertyName("DrawMatchType")]
		public string DrawMatchType { get; set; } = string.Empty;

		[JsonPropertyName("PlayerCountryA")]
		public string PlayerCountryA { get; set; } = string.Empty;

		[JsonPropertyName("PlayerCountryA2")]
		public string PlayerCountryA2 { get; set; } = string.Empty;

		[JsonPropertyName("PlayerCountryB")]
		public string PlayerCountryB { get; set; } = string.Empty;

		[JsonPropertyName("PlayerCountryB2")]
		public string PlayerCountryB2 { get; set; } = string.Empty;

		[JsonPropertyName("PlayerNameFirstA")]
		public string PlayerNameFirstA { get; set; } = string.Empty;

		[JsonPropertyName("PlayerNameFirstA2")]
		public string PlayerNameFirstA2 { get; set; } = string.Empty;

		[JsonPropertyName("PlayerNameFirstB")]
		public string PlayerNameFirstB { get; set; } = string.Empty;

		[JsonPropertyName("PlayerNameFirstB2")]
		public string PlayerNameFirstB2 { get; set; } = string.Empty;

		[JsonPropertyName("PlayerNameLastA")]
		public string PlayerNameLastA { get; set; } = string.Empty;

		[JsonPropertyName("PlayerNameLastA2")]
		public string PlayerNameLastA2 { get; set; } = string.Empty;

		[JsonPropertyName("PlayerNameLastB")]
		public string PlayerNameLastB { get; set; } = string.Empty;

		[JsonPropertyName("PlayerNameLastB2")]
		public string PlayerNameLastB2 { get; set; } = string.Empty;

		[JsonPropertyName("Winner")]
		public string Winner { get; set; } = string.Empty;

		[JsonPropertyName("ScoreString")]
		public string ScoreString { get; set; } = string.Empty;

		[JsonPropertyName("DrawLevelType")]
		public string DrawLevelType { get; set; } = string.Empty;

		[JsonPropertyName("RoundID")]
		public object RoundID { get; set; } = string.Empty;

		[JsonPropertyName("ScoreSet1A")]
		public string ScoreSet1A { get; set; } = string.Empty;

		[JsonPropertyName("ScoreSet1B")]
		public string ScoreSet1B { get; set; } = string.Empty;

		[JsonPropertyName("ScoreSet2A")]
		public string ScoreSet2A { get; set; } = string.Empty;

		[JsonPropertyName("ScoreSet2B")]
		public string ScoreSet2B { get; set; } = string.Empty;

		[JsonPropertyName("ScoreSet3A")]
		public string ScoreSet3A { get; set; } = string.Empty;

		[JsonPropertyName("ScoreSet3B")]
		public string ScoreSet3B { get; set; } = string.Empty;

		[JsonPropertyName("ScoreSet4A")]
		public string ScoreSet4A { get; set; } = string.Empty;

		[JsonPropertyName("ScoreSet4B")]
		public string ScoreSet4B { get; set; } = string.Empty;

		[JsonPropertyName("ScoreSet5A")]
		public string ScoreSet5A { get; set; } = string.Empty;

		[JsonPropertyName("ScoreSet5B")]
		public string ScoreSet5B { get; set; } = string.Empty;
	}

}
