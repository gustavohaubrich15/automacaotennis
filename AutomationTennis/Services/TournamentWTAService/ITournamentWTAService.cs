using AutomationTennis.Domain;
using AutomationTennis.DTO;

namespace AutomationTennis.Services.TournamentWTAService
{
    public interface ITournamentWTAService
    {
        Task<ResponseModel<IEnumerable<TournamentWTA>>> GetAllTournamentWTA();

        Task<ResponseModel<TournamentWTA>> GetTournamentWTAById(int id);

        Task AddListTournamentOfMonthWTAFromGenericApi();

        Task SendTournamentListOfMonthToSlackChannelWTA();
    }
}
