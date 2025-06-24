using AutomationTennis.Context;
using AutomationTennis.Filters;
using AutomationTennis.Repositories.TournamentWTARepository;
using AutomationTennis.Services.BackgroundJobService;
using AutomationTennis.Services.GenericApiService;
using AutomationTennis.Services.MatchDayWTAService;
using AutomationTennis.Services.SlackService;
using AutomationTennis.Services.TournamentWTAService;
using Hangfire;
using Hangfire.Dashboard;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<CustomExceptionFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();


//hangire
builder.Services.AddHangfire(configuration => configuration
    .UseSimpleAssemblyNameTypeSerializer().UseRecommendedSerializerSettings().UseInMemoryStorage());
builder.Services.AddHangfireServer();
builder.Services.AddHostedService<BackgroundJobService>();

//context
var dbPath = Path.Combine(AppContext.BaseDirectory,"Context", "automationTennis.db");
builder.Services.AddDbContext<AutomationTennisContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

//services and repository
builder.Services.AddScoped<IGenericApiService, GenericApiService>();
builder.Services.AddScoped<ISlackService, SlackService>();
builder.Services.AddScoped<IMatchDayWTAService, MatchDayWTAService>();
builder.Services.AddScoped<ITournamentWTAService, TournamentWTAService>();
builder.Services.AddScoped<ITournamentWTARepository, TournamentWTARepository>();


var app = builder.Build();

if (args.Contains("run-api-service-github-actions"))
{
    using var scope = app.Services.CreateScope();
    var tournamentWTAService = scope.ServiceProvider.GetRequiredService<ITournamentWTAService>();
    await tournamentWTAService.AddListTournamentOfMonthWTAFromGenericApi();
    if(DateTime.Now.Day == 1)
    {
        await tournamentWTAService.SendTournamentListOfMonthToSlackChannelWTA();
    }
    var matchDayWTAService = scope.ServiceProvider.GetRequiredService<IMatchDayWTAService>();
    await matchDayWTAService.SendMatchListOfDayToSlackChannelWTA();
    return;
}

if (app.Environment.IsDevelopment())
{
    app.UseHangfireDashboard("/hangfire");
}
else
{
    app.UseHangfireDashboard("/hangfire", new DashboardOptions
    {
        Authorization = Enumerable.Empty<IDashboardAuthorizationFilter>(),
        IsReadOnlyFunc = context => true
    });
}


app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.UseHttpsRedirection();

app.UseAuthorization();

app.Run();
