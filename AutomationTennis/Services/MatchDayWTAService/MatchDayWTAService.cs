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
                    var listMatches = new List<Match>();
                    var responseMatchDayWTA = await GetMatchDayTournamentWTAFromGenericApi(tournament.TournamentGroupWTA.IdTournamentGroupWTAIntegration);
					if(responseMatchDayWTA != null && responseMatchDayWTA.Matches.Count() > 0)
                    {
                        listMatches = responseMatchDayWTA.Matches;
                    }
					var matchChunks = SplitMatchesInChunks(listMatches, 10);

					foreach (var chunk in matchChunks)
					{
					    var formatMessage = await GenerateSlackMessage(tournament, chunk);
						_logger.LogInformation($"Enviando mensagem para slack sobre partidas do dia no canal WTA em {DateTime.Now}");
						await _slackService.SendMessageSlackForChannelWTAAsync(formatMessage);
						_logger.LogInformation($"Mensagem enviada com sucesso sobre partidas do dia para slack no canal WTA em {DateTime.Now}");
					}
				}
            }
        }

		private List<List<Match>> SplitMatchesInChunks(List<Match> matches, int chunkSize)
		{
			var matchChunks = new List<List<Match>>();
			for (int i = 0; i < matches.Count; i += chunkSize)
			{
				matchChunks.Add(matches.Skip(i).Take(chunkSize).ToList());
			}
			return matchChunks;
		}

		private async Task<object> GenerateSlackMessage(TournamentWTA tournament, List<Match> matches)
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

            if (matches.Count() > 0)
            {
                foreach (var match in matches)
                {
                    if(match.SportEvent == null || match.SportEvent.Competitors.Count == 0)
                    {
                        continue;
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

                    if(match.SportEvent.Competitors.Count() != 2)
                    {
                        continue;
                    }

					if (match.SportEvent.SportEventContext.Competition.Type == "singles" )
					{
                        var playerA = match.SportEvent.Competitors[0];

                        var playerB = match.SportEvent.Competitors[1];

						contextBlock.Elements.Add(new MrkdwnText
						{
							Text = $"{CountryFlagSlack(playerA.CountryCode)} {playerA.Name} *vs* {CountryFlagSlack(playerB.CountryCode)} {playerB.Name}"
						});
					}
					else if (match.SportEvent.Competitors[0].Players != null && match.SportEvent.Competitors[0].Players?.Count() == 2
							&& match.SportEvent.Competitors[1].Players != null && match.SportEvent.Competitors[1].Players?.Count() == 2 )
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
			}
            else
            {
                var contextBlock = new ContextBlock();
                var dividerBlock = new DividerBlock();
                blockKit.Blocks.Add(dividerBlock);
                contextBlock.Elements.Add(new MrkdwnText { Text = $"*Nenhuma partida para hoje.*" });
                blockKit.Blocks.Add(contextBlock);
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

        private string CountryFlagSlack(string playerCountry)
        {
			if (string.IsNullOrWhiteSpace(playerCountry))
			{
				return "";
			}
			var playerCountryLower = CountryNameAdjuster.AdjustCountryName(playerCountry);
            return $":flag-{playerCountryLower.Substring(0, 2)}:";
        }

        public async Task<ResponseApiMatchDayWTA?> GetMatchDayTournamentWTAFromGenericApi(int idTournamentInegration)
        {
            AdjustDateApiWta();
            var UrlWtaMatchAdjust = UrlWtaMatch.Replace("idTournament", idTournamentInegration.ToString())
                                               .Replace("year", DateTime.Now.Year.ToString());
            _logger.LogInformation($"Iniciando chamada api de torneios WTA em {DateTime.Now}");
            var response = await _genericApiService.GetAsync<ResponseApiMatchDayWTA>(UrlWtaMatchAdjust);
            _logger.LogInformation($"Chamada com sucesso. Resposta da chamada api de torneios WTA : {response}");
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
