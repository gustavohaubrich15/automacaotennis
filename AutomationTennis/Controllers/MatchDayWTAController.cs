using AutomationTennis.Services.MatchDayWTAService;
using Microsoft.AspNetCore.Mvc;

namespace AutomationTennis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchDayWTAController : ControllerBase
    {
        private readonly ILogger<MatchDayWTAController> _logger;
        private readonly IMatchDayWTAService _matchDayWTAService;

        public MatchDayWTAController(ILogger<MatchDayWTAController> logger,
            IMatchDayWTAService matchDayWTAService)
        {
            _logger = logger;
            _matchDayWTAService = matchDayWTAService;
        }

        [HttpPost("SendMatchListOfDayToSlackChannelWTA")]
        public async Task<ActionResult> SendMatchListOfDayToSlackChannelWTA()
        {
            await _matchDayWTAService.SendMatchListOfDayToSlackChannelWTA();
            return Ok();
        }
    }
}
