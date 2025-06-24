using AutomationTennis.BlockKitSlack;
using AutomationTennis.Domain;
using AutomationTennis.DTO;
using AutomationTennis.Exceptions;
using AutomationTennis.Repositories.TournamentWTARepository;
using AutomationTennis.Response;
using AutomationTennis.Services.GenericApiService;
using AutomationTennis.Services.SlackService;

namespace AutomationTennis.Services.TournamentWTAService
{
    public class TournamentWTAService : ITournamentWTAService
    {
        private readonly ILogger<TournamentWTAService> _logger;
        private readonly ITournamentWTARepository _tournamentWTARepository;
        private readonly ISlackService _slackService;
        private readonly IGenericApiService _genericApiService;
        private readonly IConfiguration _configuration;
        private string FromDateAPIWTA { get; set; } = string.Empty;
        private string ToDateAPIWTA { get; set; } = string.Empty;
        private string UrlWtaTournament => $"{_configuration["WTA:TournamentApi"]}&from={FromDateAPIWTA}&to={ToDateAPIWTA}";

        public TournamentWTAService(ILogger<TournamentWTAService> logger,
            ITournamentWTARepository tournamentWTARepository,
            ISlackService slackService,
            IGenericApiService genericApiService,
            IConfiguration configuration)
        {

            _logger = logger;
            _tournamentWTARepository = tournamentWTARepository;
            _genericApiService = genericApiService;
            _slackService = slackService;
            _configuration = configuration;
            var wtaApi = _configuration["WTA:TournamentApi"];
            _logger.LogInformation($"Valor lido de WTA:TournamentApi no construtor: {wtaApi ?? "null"}");
        }

        public async Task AddListTournamentOfMonthWTAFromGenericApi()
        {
            AdjustDateApiWta();
            _logger.LogInformation($"Iniciando chamada api de torneios WTA em {DateTime.Now}");
            _logger.LogInformation($"Url api de torneios WTA em {UrlWtaTournament}");
            var response = await _genericApiService.GetAsync<ResponseApiTournamentWTA>(UrlWtaTournament);
            _logger.LogInformation($"Chamada com sucesso. Resposta da chamada api de torneios WTA : {response}");
            var mappedTournament = MappedResponse(response);
            _logger.LogInformation($"Iniciando inclusão de torneios WTA no banco em {DateTime.Now}");
            AddTournamentWTAInRepository(mappedTournament);
            _logger.LogInformation($"Inclusão de torneios no banco com sucesso em {DateTime.Now}");
        }

        private void AdjustDateApiWta()
        {
            var currentDate = DateTime.Now;
            FromDateAPIWTA = $"{currentDate.Year}-{currentDate.Month:D2}-01";
            ToDateAPIWTA = $"{currentDate.Year}-{currentDate.Month:D2}-{DateTime.DaysInMonth(currentDate.Year, currentDate.Month):D2}";
        }

        private IEnumerable<TournamentWTA> MappedResponse(ResponseApiTournamentWTA? response)
        {
            var tournaments = new List<TournamentWTA>();
            if (response != null)
            {
                tournaments = response.Content.Select(tournament => new TournamentWTA
                {
                    Description = tournament.Title,
                    Surface = tournament.Surface.ToLower(),
                    SinglesDrawSize = tournament.SinglesDrawSize,
                    DoublesDrawSize = tournament.DoublesDrawSize,
                    InOutdoor = tournament.InOutdoor.ToLower() switch
                    {
                        "i" => "Indoor",
                        "o" => "Outdoor",
                        _ => "Desconhecido"
                    },
                    PrizeMoney = tournament.PrizeMoney,
                    PrizeMoneyCurrency = tournament.PrizeMoneyCurrency,
                    StartDate = tournament.StartDate,
                    EndDate = tournament.EndDate,
                    TournamentGroupWTA = new TournamentGroupWTA
                    {
                        IdTournamentGroupWTAIntegration = tournament.TournamentGroup.Id,
                        Level = tournament.TournamentGroup.Level,
                        Name = tournament.TournamentGroup.Name,
                    }
                }).ToList();
            }
            return tournaments;
        }

        private async void AddTournamentWTAInRepository(IEnumerable<TournamentWTA> newTournamentsWTA)
        {
            _logger.LogInformation($"Removendo todos os torneios WTA na base em {DateTime.Now}");
            await _tournamentWTARepository.DeleteAllAsync();
            _logger.LogInformation($"Incluindo os torneios do mês {DateTime.Now.Month.ToString()} da WTA na base em {DateTime.Now}");
            await _tournamentWTARepository.CreateRangeAsync(newTournamentsWTA);
        }

        public async Task<ResponseModel<IEnumerable<TournamentWTA>>> GetAllTournamentWTA()
        {
            var tournamentList = await _tournamentWTARepository.GetAllAsync(t => t.TournamentGroupWTA);
            ResponseModel<IEnumerable<TournamentWTA>> response = new ResponseModel<IEnumerable<TournamentWTA>>();
            response.Data = tournamentList;
            return response;
        }

        public async Task<ResponseModel<TournamentWTA>> GetTournamentWTAById(int id)
        {
            var tournament = await _tournamentWTARepository.GetByIdAsync(id);
            if (tournament == null)
            {
                throw new BusinessException(BusinessExceptionMessage.RegisterNotFound);
            }
            ResponseModel<TournamentWTA> response = new ResponseModel<TournamentWTA>();
            response.Data = tournament;
            return response;
        }

        public async Task SendTournamentListOfMonthToSlackChannelWTA()
        {
            var responseTournamentList = await GetAllTournamentWTA();
            var tournamentList = responseTournamentList.Data;

            foreach(var tournament in tournamentList)
            {
                var formatMessage = GenerateSlackMessage(tournament);
                _logger.LogInformation($"Enviando mensagem para slack sobre torneios do mês no canal WTA em {DateTime.Now}");
                await _slackService.SendMessageSlackForChannelWTAAsync(formatMessage);
                _logger.LogInformation($"Mensagem enviada com sucesso para slack sobre torneios do mês no canal WTA em {DateTime.Now}");
            }
         
        }

        private object GenerateSlackMessage(TournamentWTA tournament)
        {
            var headerBlock = new HeaderBlock
            {
                Text = new PlainText
                {
                    Text = $"{tournament.TournamentGroupWTA.Level} - {tournament.Description} :tennis:"
                }
            };

            var contextBlock = new ContextBlock();
            if(tournament.TournamentGroupWTA != null)
            {
                contextBlock.Elements.Add(new MrkdwnText { Text = $"*Nível:* {tournament.TournamentGroupWTA.Level}" });
            }
            contextBlock.Elements.Add(new MrkdwnText { Text = $"*Início:* {ConvertToBrazilianDate(tournament.StartDate)}" });
            contextBlock.Elements.Add(new MrkdwnText { Text = $"*Fim:* {ConvertToBrazilianDate(tournament.EndDate)}" });
            contextBlock.Elements.Add(new MrkdwnText { Text = $"*Ambiente:* {tournament.InOutdoor}" });
            contextBlock.Elements.Add(new MrkdwnText { Text = $"*Chave de simples:* {tournament.SinglesDrawSize}" });
            contextBlock.Elements.Add(new MrkdwnText { Text = $"*Chave de duplas:* {tournament.DoublesDrawSize}" });
            contextBlock.Elements.Add(new MrkdwnText { Text = $":moneybag: *Premiação:* {tournament.PrizeMoney.ToString("N2")} *{tournament.PrizeMoneyCurrency}*" });

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

            var dividerBlock = new DividerBlock();

            var blockKit = new BlockKit();
            blockKit.Blocks.Add(headerBlock);
            blockKit.Blocks.Add(contextBlock);
            blockKit.Blocks.Add(imageBlock);
            blockKit.Blocks.Add(dividerBlock);

            var slackMessage = blockKit;
            return slackMessage;
          }

        
        private string ConvertToBrazilianDate(string stringDateFormat)
        {
            DateTime dateFormat = DateTime.Parse(stringDateFormat);
            return dateFormat.ToString("dd/MM/yyyy");
        }

    }
}
