namespace AutomationTennis.Services.SlackService
{
    public interface ISlackService
    {
        Task SendMessageSlackForChannelWTAAsync(object slackMessage);
    }
}
