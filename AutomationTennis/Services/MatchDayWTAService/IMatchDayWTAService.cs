namespace AutomationTennis.Services.MatchDayWTAService
{
    public interface IMatchDayWTAService
    {
        Task SendMatchListOfDayToSlackChannelWTA();
    }
}
