using AutomationTennis.Services.MatchDayWTAService;
using AutomationTennis.Services.TournamentWTAService;
using Hangfire;

namespace AutomationTennis.Services.BackgroundJobService
{
    public class BackgroundJobService : IHostedService
    {
        private readonly ILogger<BackgroundJobService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public BackgroundJobService(ILogger<BackgroundJobService> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Serviço de background começou a rodar em {DateTime.Now}");
            AddRecurringJob();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Serviço de background parou em {DateTime.Now}");
            return Task.CompletedTask;
        }

        private void AddRecurringJob()
        {
            _logger.LogInformation("Adicionando tarefas recorrentes");
            RecurringJob.AddOrUpdate("AddListTournamentOfMonthWTAFromGenericApi", () => AddListTournamentOfMonthWTAFromGenericApi(), Monthly(1,9,30));
            RecurringJob.AddOrUpdate("SendTournamentListOfMonthToSlackChannelWTA", () => SendTournamentListOfMonthToSlackChannelWTA(), Monthly(1,11,0));
            RecurringJob.AddOrUpdate("SendMatchListOfDayToSlackChannelWTA", () => SendMatchListOfDayToSlackChannelWTA(), Daily(12,0));
        }

        public void AddListTournamentOfMonthWTAFromGenericApi()
        {
            _logger.LogInformation("Executando a tarefa -  AddListTournamentOfMonthWTAFromGenericApi");
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var tournamentWTAService = scope.ServiceProvider.GetRequiredService<ITournamentWTAService>();
                    tournamentWTAService.AddListTournamentOfMonthWTAFromGenericApi().Wait();
                }
                _logger.LogInformation("Finalizando com sucesso a tarefa -  AddListTournamentOfMonthWTAFromGenericApi");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante a execução da tarefa agendada.");
            }
        }

        public void SendTournamentListOfMonthToSlackChannelWTA()
        {
            _logger.LogInformation("Executando a tarefa -  SendTournamentListOfMonthToSlackChannelWTA");
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var tournamentWTAService = scope.ServiceProvider.GetRequiredService<ITournamentWTAService>();
                    tournamentWTAService.SendTournamentListOfMonthToSlackChannelWTA().Wait();
                }
                _logger.LogInformation("Finalizando com sucesso a tarefa -  SendTournamentListOfMonthToSlackChannelWTA()");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante a execução da tarefa agendada.");
            }
        }

        public void SendMatchListOfDayToSlackChannelWTA()
        {
            _logger.LogInformation("Executando a tarefa -  SendMatchListOfDayToSlackChannelWTA");
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var matchDayWTAService = scope.ServiceProvider.GetRequiredService<IMatchDayWTAService>();
                    matchDayWTAService.SendMatchListOfDayToSlackChannelWTA().Wait();
                }
                _logger.LogInformation("Finalizando com sucesso a tarefa -  SendMatchListOfDayToSlackChannelWTA()");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro durante a execução da tarefa agendada.");
            }
        }

        private string Monthly(int day, int hour, int minute)
        {
            return $"{minute} {hour} {day} * *";
        }

        private string Daily(int hour, int minute)
        {
            return $"{minute} {hour} * * *";
        }

        private string MinuteInterval(int minute)
        {
            return $"*/{minute} * * * *";
        }

    }
}
