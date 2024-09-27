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
                    var formatMessage = await GenerateSlackMessage(tournament);
                    _logger.LogInformation($"Enviando mensagem para slack sobre partida do dia no canal WTA em {DateTime.Now}");
                    await _slackService.SendMessageSlackForChannelWTAAsync(formatMessage);
                    _logger.LogInformation($"Mensagem enviada com sucesso sobre partida do dia  para slack no canal WTA em {DateTime.Now}");
                }
            }
        }

        private async Task<object> GenerateSlackMessage(TournamentWTA tournament)
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

            var responseMatchDayWTA = await GetMatchDayTournamentWTAFromGenericApi(tournament.TournamentGroupWTA.IdTournamentGroupWTAIntegration);
            if (responseMatchDayWTA != null)
            {
                foreach (var match in responseMatchDayWTA.Matches)
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
                                       ? $", {match.ScoreSet2A}-{match.ScoreSet2B}" + (!string.IsNullOrEmpty(match.ScoreSet3A) && !string.IsNullOrEmpty(match.ScoreSet3B) ? $", {match.ScoreSet3A}-{match.ScoreSet3B}": string.Empty): string.Empty)
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
