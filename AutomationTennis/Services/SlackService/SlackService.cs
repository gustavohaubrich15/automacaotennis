using AutomationTennis.Exceptions;
using AutomationTennis.Services.GenericApiService;

namespace AutomationTennis.Services.SlackService
{
    public class SlackService : ISlackService
    {
        private readonly ILogger<SlackService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IGenericApiService _genericApiService;

        public SlackService(ILogger<SlackService> logger,
            IConfiguration configuration,
            IGenericApiService genericApiService)
        {
            _logger = logger;
            _configuration = configuration;
            _genericApiService = genericApiService;
        }

        public async Task SendMessageSlackForChannelWTAAsync(object slackMessage)
        {
            var webhookUrlWebhookUrlChannelWTA = _configuration["Slack:WebhookUrlChannelWTA"];
            if (string.IsNullOrEmpty(webhookUrlWebhookUrlChannelWTA))
            {
                throw new BusinessException(BusinessExceptionMessage.SlackWebhookUrlChannelWTANotFound);
            }
            var body = slackMessage;
            _logger.LogInformation($"Iniciando chamada webhook slack para o canal WTA em {DateTime.Now}");
            var response = await _genericApiService.PostAsync<object,string>(webhookUrlWebhookUrlChannelWTA, body);
            _logger.LogInformation($"Enviado com sucesso chamada webhook slack para o canal WTA em {DateTime.Now}");

        }

    }
}
