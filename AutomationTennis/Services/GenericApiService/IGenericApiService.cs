namespace AutomationTennis.Services.GenericApiService
{
    public interface IGenericApiService
    {
        Task<TResponse?> GetAsync<TResponse>(string url);
        Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest requestBody);
    }
}
