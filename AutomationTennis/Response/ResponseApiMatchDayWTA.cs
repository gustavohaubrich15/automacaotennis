using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace AutomationTennis.Response
{
	[Serializable]
	public class ResponseApiMatchDayWTA
	{
		[JsonPropertyName("matches")]
		public List<Match> Matches { get; set; } = new List<Match>();
	}

	[Serializable]
	public class Match
	{
		[JsonPropertyName("sport_event")]
		public SportEvent SportEvent { get; set; } = new SportEvent();

		[JsonPropertyName("sport_event_status")]
		public SportEventStatus SportEventStatus { get; set; } = new SportEventStatus();
	}

	[Serializable]
	public class SportEvent
	{
		[JsonPropertyName("id")]
		public string Id { get; set; } = string.Empty;

		[JsonPropertyName("start_time")]
		public DateTime StartTime { get; set; }

		[JsonPropertyName("start_time_confirmed")]
		public bool StartTimeConfirmed { get; set; }

		[JsonPropertyName("sport_event_context")]
		public SportEventContext SportEventContext { get; set; } = new SportEventContext();

		[JsonPropertyName("coverage")]
		public Coverage Coverage { get; set; } = new Coverage();

		[JsonPropertyName("competitors")]
		public List<Competitor> Competitors { get; set; } = new List<Competitor>();

		[JsonPropertyName("venue")]
		public Venue Venue { get; set; } = new Venue();

		[JsonPropertyName("estimated")]
		public bool Estimated { get; set; }
	}

	[Serializable]
	public class SportEventContext
	{
		[JsonPropertyName("sport")]
		public ContextItem Sport { get; set; } = new ContextItem();

		[JsonPropertyName("category")]
		public ContextItem Category { get; set; } = new ContextItem();

		[JsonPropertyName("competition")]
		public Competition Competition { get; set; } = new Competition();

		[JsonPropertyName("season")]
		public Season Season { get; set; } = new Season();

		[JsonPropertyName("stage")]
		public Stage Stage { get; set; } = new Stage();

		[JsonPropertyName("round")]
		public Round Round { get; set; } = new Round();

		[JsonPropertyName("groups")]
		public List<Group> Groups { get; set; } = new List<Group>();

		[JsonPropertyName("mode")]
		public Mode Mode { get; set; } = new Mode();
	}

	[Serializable]
	public class ContextItem
	{
		[JsonPropertyName("id")]
		public string Id { get; set; } = string.Empty;

		[JsonPropertyName("name")]
		public string Name { get; set; } = string.Empty;
	}

	[Serializable]
	public class Competition : ContextItem
	{
		[JsonPropertyName("parent_id")]
		public string ParentId { get; set; } = string.Empty;

		[JsonPropertyName("type")]
		public string Type { get; set; } = string.Empty;

		[JsonPropertyName("gender")]
		public string Gender { get; set; } = string.Empty;

		[JsonPropertyName("level")]
		public string Level { get; set; } = string.Empty;
	}

	[Serializable]
	public class Season : ContextItem
	{
		[JsonPropertyName("start_date")]
		public string StartDate { get; set; } = string.Empty;

		[JsonPropertyName("end_date")]
		public string EndDate { get; set; } = string.Empty;

		[JsonPropertyName("year")]
		public string Year { get; set; } = string.Empty;

		[JsonPropertyName("competition_id")]
		public string CompetitionId { get; set; } = string.Empty;
	}

	[Serializable]
	public class Stage
	{
		[JsonPropertyName("order")]
		public int Order { get; set; }

		[JsonPropertyName("type")]
		public string Type { get; set; } = string.Empty;

		[JsonPropertyName("phase")]
		public string Phase { get; set; } = string.Empty;

		[JsonPropertyName("start_date")]
		public string StartDate { get; set; } = string.Empty;

		[JsonPropertyName("end_date")]
		public string EndDate { get; set; } = string.Empty;

		[JsonPropertyName("year")]
		public string Year { get; set; } = string.Empty;
	}

	[Serializable]
	public class Round
	{
		[JsonPropertyName("name")]
		public string Name { get; set; } = string.Empty;
	}

	[Serializable]
	public class Group
	{
		[JsonPropertyName("id")]
		public string Id { get; set; } = string.Empty;

		[JsonPropertyName("name")]
		public string Name { get; set; } = string.Empty;
	}

	[Serializable]
	public class Mode
	{
		[JsonPropertyName("best_of")]
		public int BestOf { get; set; }
	}

	[Serializable]
	public class Coverage
	{
		[JsonPropertyName("type")]
		public string Type { get; set; } = string.Empty;

		[JsonPropertyName("sport_event_properties")]
		public SportEventProperties SportEventProperties { get; set; } = new SportEventProperties();
	}

	[Serializable]
	public class SportEventProperties
	{
		[JsonPropertyName("enhanced_stats")]
		public bool EnhancedStats { get; set; }

		[JsonPropertyName("scores")]
		public string Scores { get; set; } = string.Empty;

		[JsonPropertyName("detailed_serve_outcomes")]
		public bool DetailedServeOutcomes { get; set; }

		[JsonPropertyName("play_by_play")]
		public bool PlayByPlay { get; set; }
	}

	[Serializable]
	public class Competitor
	{
		[JsonPropertyName("id")]
		public string Id { get; set; } = string.Empty;

		[JsonPropertyName("name")]
		public string Name { get; set; } = string.Empty;

		[JsonPropertyName("country")]
		public string? Country { get; set; } // Opcional para duplas

		[JsonPropertyName("country_code")]
		public string? CountryCode { get; set; } // Opcional para duplas

		[JsonPropertyName("abbreviation")]
		public string Abbreviation { get; set; } = string.Empty;

		[JsonPropertyName("qualifier")]
		public string Qualifier { get; set; } = string.Empty;

		[JsonPropertyName("seed")]
		public int? Seed { get; set; } // Opcional para simples

		[JsonPropertyName("bracket_number")]
		public int BracketNumber { get; set; }

		[JsonPropertyName("players")]
		public List<Player>? Players { get; set; } // Presente apenas em duplas
	}

	public class Player
	{
		[JsonPropertyName("id")]
		public string Id { get; set; } = string.Empty;

		[JsonPropertyName("name")]
		public string Name { get; set; } = string.Empty;

		[JsonPropertyName("country")]
		public string Country { get; set; } = string.Empty;

		[JsonPropertyName("country_code")]
		public string CountryCode { get; set; } = string.Empty;

		[JsonPropertyName("abbreviation")]
		public string Abbreviation { get; set; } = string.Empty;
	}



	[Serializable]
	public class Venue
	{
		[JsonPropertyName("id")]
		public string Id { get; set; } = string.Empty;

		[JsonPropertyName("name")]
		public string Name { get; set; } = string.Empty;

		[JsonPropertyName("city_name")]
		public string CityName { get; set; } = string.Empty;

		[JsonPropertyName("country_name")]
		public string CountryName { get; set; } = string.Empty;

		[JsonPropertyName("country_code")]
		public string CountryCode { get; set; } = string.Empty;

		[JsonPropertyName("timezone")]
		public string Timezone { get; set; } = string.Empty;
	}

	[Serializable]
	public class SportEventStatus
	{
		[JsonPropertyName("status")]
		public string Status { get; set; } = string.Empty;

		[JsonPropertyName("match_status")]
		public string MatchStatus { get; set; } = string.Empty;

		[JsonPropertyName("period_scores")]
		public List<PeriodScore> PeriodScore { get; set; }
	}

	public class PeriodScore
	{
		[JsonPropertyName("home_score")]
		public int HomeScore { get; set; }

		[JsonPropertyName("away_score")]
		public int AwayScore { get; set; }

		[JsonPropertyName("type")]
		public string Type { get; set; } = string.Empty;

		[JsonPropertyName("number")]
		public int Number { get; set; }

		[JsonPropertyName("home_tiebreak_score")]
		public int? HomeTiebreakScore { get; set; }

		[JsonPropertyName("away_tiebreak_score")]
		public int? AwayTiebreakScore { get; set; }
	}
}
