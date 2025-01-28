using AutomationTennis.BlockKitSlack;
using AutomationTennis.Domain;
using AutomationTennis.Response;
using AutomationTennis.Services.GenericApiService;
using AutomationTennis.Services.SlackService;
using AutomationTennis.Services.TournamentWTAService;
using AutomationTennis.Utils;

namespace AutomationTennis.Services.MatchDayWTAService
{
	public class MatchDayWTAService : IMatchDayWTAService
	{
		private readonly ILogger<MatchDayWTAService> _logger;
		private readonly ITournamentWTAService _tournamentWTAService;
		private readonly ISlackService _slackService;
		private readonly IGenericApiService _genericApiService;
		private readonly IConfiguration _configuration;
		private string FromDateAPIWTA { get; set; } = string.Empty;
		private string ToDateAPIWTA { get; set; } = string.Empty;
		private string UrlWtaMatch => $"{_configuration["WTA:MatchApi"]}&from={FromDateAPIWTA}&to={ToDateAPIWTA}";

		public MatchDayWTAService(ILogger<MatchDayWTAService> logger,
			ITournamentWTAService tournamentWTAService,
			ISlackService slackService,
			IGenericApiService genericApiService,
			IConfiguration configuration)
		{

			_logger = logger;
			_tournamentWTAService = tournamentWTAService;
			_genericApiService = genericApiService;
			_slackService = slackService;
			_configuration = configuration;
		}

		public async Task SendMatchListOfDayToSlackChannelWTA()
		{
			var responseTournamentList = await _tournamentWTAService.GetAllTournamentWTA();
			var tournamentList = responseTournamentList.Data;

			if (tournamentList != null)
			{
				tournamentList = tournamentList.Where(a => DateTime.Parse(a.StartDate).Date <= DateTime.Now.Date
													&& DateTime.Parse(a.EndDate).Date >= DateTime.Now.Date)
											 .ToList();
				foreach (var tournament in tournamentList)
				{
					_logger.LogInformation($"Processando torneio: {tournament.Description} - {tournament.TournamentGroupWTA.Level}");
					await ProcessTournamentFormat(tournament);
				}
			}
		}

		private async Task ProcessTournamentFormat(TournamentWTA tournament)
		{
			var responseMatchDayWTAFormat1 = await GetMatchDayTournamentWTAFromGenericApi<ResponseApiMatchDayWTAFormat1>(tournament.TournamentGroupWTA.IdTournamentGroupWTAIntegration);
			if (responseMatchDayWTAFormat1 != null && responseMatchDayWTAFormat1.Matches.Any() && responseMatchDayWTAFormat1.Matches[0].SportEvent.Competitors.Count() > 0)
			{
				_logger.LogInformation($"Formato 1 utilizado para o torneio: {tournament.Description}. Total de partidas: {responseMatchDayWTAFormat1.Matches.Count()}");
				var responseMatchesFormat1 = responseMatchDayWTAFormat1.Matches;
				var matchChunks = SplitMatchesInChunks(responseMatchesFormat1, 10);
				foreach (var chunk in matchChunks)
				{
					var formatMessage = await GenerateSlackMessage(tournament, chunk);
					_logger.LogInformation($"Enviando mensagem para Slack sobre partidas do torneio: {tournament.Description} em {DateTime.Now}");
					await _slackService.SendMessageSlackForChannelWTAAsync(formatMessage);
					_logger.LogInformation($"Mensagem enviada com sucesso para Slack sobre partidas do torneio: {tournament.Description} em {DateTime.Now}");
				}
				return;
			}

			var responseMatchDayWTAFormat2 = await GetMatchDayTournamentWTAFromGenericApi<ResponseApiMatchDayWTAFormat2>(tournament.TournamentGroupWTA.IdTournamentGroupWTAIntegration);
			if (responseMatchDayWTAFormat2 != null && responseMatchDayWTAFormat2.Matches.Any() && !string.IsNullOrEmpty(responseMatchDayWTAFormat2.Matches[0].PlayerNameFirstA))
			{
				_logger.LogInformation($"Formato 2 utilizado para o torneio: {tournament.Description}. Total de partidas: {responseMatchDayWTAFormat2.Matches.Count()}");
				var responseMatches = responseMatchDayWTAFormat2.Matches;
				var matchChunks = SplitMatchesInChunks(responseMatches, 10);
				foreach (var chunk in matchChunks)
				{
					var formatMessage = await GenerateSlackMessage(tournament, chunk);
					_logger.LogInformation($"Enviando mensagem para Slack sobre partidas do torneio: {tournament.Description} em {DateTime.Now}");
					await _slackService.SendMessageSlackForChannelWTAAsync(formatMessage);
					_logger.LogInformation($"Mensagem enviada com sucesso para Slack sobre partidas do torneio: {tournament.Description} em {DateTime.Now}");
				}
			}
			else
			{
				_logger.LogInformation($"Nenhuma partida encontrada para o torneio: {tournament.Description} nos formatos de resposta disponíveis.");
			}
		}

		private List<List<T>> SplitMatchesInChunks<T>(List<T> items, int chunkSize)
		{
			var chunks = new List<List<T>>();
			for (int i = 0; i < items.Count; i += chunkSize)
			{
				chunks.Add(items.Skip(i).Take(chunkSize).ToList());
			}
			return chunks;
		}


		private async Task<object> GenerateSlackMessage(TournamentWTA tournament, dynamic matches)
		{
			var blockKit = new BlockKit();
			var headerBlock = new HeaderBlock
			{
				Text = new PlainText
				{
					Text = $"Programação  {DateTime.Now.ToString("dd/MM/yyyy")} - {tournament.TournamentGroupWTA.Level} - {tournament.Description} :tennis:"
				}
			};
			blockKit.Blocks.Add(headerBlock);

			foreach (var match in matches)
			{
				if (match is MatchFormat1)
				{
					GenerateSlackMessageMatchesFormat1(blockKit, (MatchFormat1)match);
				}
				else if (match is MatchFormat2)
				{
					GenerateSlackMessageMatchesFormat2(blockKit, (MatchFormat2)match);
				}
			}


			var imageBlock = new ImageBlock
			{
				BlockId = "image4",
				ImageUrl = tournament.Surface.ToLower() switch
				{
					"clay" => _configuration["Slack:ImageUrlTennisCourtClay"] ?? string.Empty,
					"grass" => _configuration["Slack:ImageUrlTennisCourtGrass"] ?? string.Empty,
					"hard" => _configuration["Slack:ImageUrlTennisCourtHard"] ?? string.Empty,
					_ => string.Empty
				},
				AltText = "Tennis Court"
			};
			blockKit.Blocks.Add(imageBlock);

			var slackMessage = blockKit;
			return slackMessage;
		}


		private void GenerateSlackMessageMatchesFormat1(BlockKit blockKit, MatchFormat1 match)
		{
			if (match.SportEvent == null || match.SportEvent.Competitors.Count == 0)
			{
				return;
			}
			var contextBlock = new ContextBlock();
			var dividerBlock = new DividerBlock();
			blockKit.Blocks.Add(dividerBlock);
			if (!string.IsNullOrEmpty(match.SportEvent.Venue.Name))
			{
				contextBlock.Elements.Add(new MrkdwnText { Text = $"*Quadra* - {match.SportEvent.Venue.Name}" });
			}

			var roundName = match.SportEvent.SportEventContext.Round.Name;

			if (!string.IsNullOrEmpty(roundName))
			{
				contextBlock.Elements.Add(new MrkdwnText { Text = $"*{roundName}*" });
			}

			var resultScore = "Partida a ser realizada";

			if (match.SportEventStatus.PeriodScore != null && match.SportEventStatus.PeriodScore.Any())
			{
				var periodScores = match.SportEventStatus.PeriodScore;
				resultScore = "*Resultado:* " + string.Join(", ", periodScores.Select(ps =>
				{
					var score = $"{ps.HomeScore}-{ps.AwayScore}";
					if (ps.Type == "set" && ps.HomeTiebreakScore.HasValue && ps.AwayTiebreakScore.HasValue)
					{
						score += $" (tiebreak: {ps.HomeTiebreakScore}-{ps.AwayTiebreakScore})";
					}
					return score;
				}));
			}

			if (match.SportEvent.Competitors.Count() != 2)
			{
				return;
			}

			if (match.SportEvent.SportEventContext.Competition.Type == "singles")
			{
				var playerA = match.SportEvent.Competitors[0];

				var playerB = match.SportEvent.Competitors[1];

				contextBlock.Elements.Add(new MrkdwnText
				{
					Text = $"{CountryFlagSlack(playerA.CountryCode)} {playerA.Name} *vs* {CountryFlagSlack(playerB.CountryCode)} {playerB.Name}"
				});
			}
			else if (match.SportEvent.Competitors[0].Players != null && match.SportEvent.Competitors[0].Players?.Count() == 2
					&& match.SportEvent.Competitors[1].Players != null && match.SportEvent.Competitors[1].Players?.Count() == 2)
			{
				var playerA1 = match.SportEvent.Competitors[0].Players?[0];
				var playerA2 = match.SportEvent.Competitors[0].Players?[1];
				var playerB1 = match.SportEvent.Competitors[1].Players?[0];
				var playerB2 = match.SportEvent.Competitors[1].Players?[1];

				contextBlock.Elements.Add(new MrkdwnText
				{
					Text = $"{CountryFlagSlack(playerA1.CountryCode)} {playerA1.Name} / {CountryFlagSlack(playerA2.CountryCode)}{playerA2.Name} *vs* {CountryFlagSlack(playerB1.CountryCode)} {playerB1.Name} / {CountryFlagSlack(playerB2.CountryCode)} {playerB2.Name}"
				});
			}

			// Adiciona a pontuação ao bloco
			contextBlock.Elements.Add(new MrkdwnText { Text = resultScore });
			blockKit.Blocks.Add(contextBlock);
		}


		private void GenerateSlackMessageMatchesFormat2(BlockKit blockKit, MatchFormat2 match)
		{
			var contextBlock = new ContextBlock();
			var dividerBlock = new DividerBlock();
			blockKit.Blocks.Add(dividerBlock);
			if (!string.IsNullOrEmpty(match.CourtName))
			{
				contextBlock.Elements.Add(new MrkdwnText { Text = $"*Quadra* - {match.CourtName}" });
			}
			var roundName = RoundNameWTA.GetRoundName(match.DrawLevelType, match.RoundID.ToString() ?? string.Empty);

			if (!string.IsNullOrEmpty(roundName))
			{
				contextBlock.Elements.Add(new MrkdwnText { Text = $"*{roundName}*" });
			}
			var resultScore = (!string.IsNullOrEmpty(match.ScoreSet1A) && !string.IsNullOrEmpty(match.ScoreSet1B)) ? $"*Resultado:* {match.ScoreSet1A}-{match.ScoreSet1B}" + (!string.IsNullOrEmpty(match.ScoreSet2A) && !string.IsNullOrEmpty(match.ScoreSet2B)
			? $", {match.ScoreSet2A}-{match.ScoreSet2B}" + (!string.IsNullOrEmpty(match.ScoreSet3A) && !string.IsNullOrEmpty(match.ScoreSet3B) ? $", {match.ScoreSet3A}-{match.ScoreSet3B}" : string.Empty) : string.Empty)
			: "Partida a ser realizada";
			if (match.DrawMatchType == "S")
			{
				contextBlock.Elements.Add(new MrkdwnText { Text = $"{CountryFlagSlack(match.PlayerCountryA)} {match.PlayerNameFirstA} {match.PlayerNameLastA} *vs* {CountryFlagSlack(match.PlayerCountryB)} {match.PlayerNameFirstB} {match.PlayerNameLastB}" });
			}
			else
			{
				contextBlock.Elements.Add(new MrkdwnText { Text = $"{CountryFlagSlack(match.PlayerCountryA)} {match.PlayerNameFirstA.Substring(0, 1)}.{match.PlayerNameLastA}/ {CountryFlagSlack(match.PlayerCountryA2)} {match.PlayerNameFirstA2.Substring(0, 1)}.{match.PlayerNameLastA2} *vs* {CountryFlagSlack(match.PlayerCountryB)} {match.PlayerNameFirstB.Substring(0, 1)}.{match.PlayerNameLastB}/ {CountryFlagSlack(match.PlayerCountryB2)} {match.PlayerNameFirstB2.Substring(0, 1)}.{match.PlayerNameLastB2}" });
			}
			contextBlock.Elements.Add(new MrkdwnText { Text = $"{resultScore}" });
			blockKit.Blocks.Add(contextBlock);
		}

		private string CountryFlagSlack(string playerCountry)
		{
			if (string.IsNullOrWhiteSpace(playerCountry))
			{
				return "";
			}
			var playerCountryLower = CountryNameAdjuster.AdjustCountryName(playerCountry);
			return $":flag-{playerCountryLower.Substring(0, 2)}:";
		}
		public async Task<T?> GetMatchDayTournamentWTAFromGenericApi<T>(int idTournamentIntegration)
		{
			AdjustDateApiWta();
			var urlWtaMatchAdjust = UrlWtaMatch.Replace("idTournament", idTournamentIntegration.ToString())
			.Replace("year", DateTime.Now.Year.ToString());
			_logger.LogInformation($"Iniciando chamada API de torneios WTA em {DateTime.Now} para o formato de response {typeof(T).Name}");
			var response = await _genericApiService.GetAsync<T>(urlWtaMatchAdjust);
			_logger.LogInformation($"Chamada com sucesso. Resposta da chamada API de torneios WTA: {response}");
			return response;
		}

		private void AdjustDateApiWta()
		{
			var currentDate = DateTime.Now;
			FromDateAPIWTA = $"{currentDate.Year}-{currentDate.Month:D2}-{currentDate.Day:D2}";
			ToDateAPIWTA = $"{currentDate.Year}-{currentDate.Month:D2}-{currentDate.Day:D2}";
		}
	}
}
