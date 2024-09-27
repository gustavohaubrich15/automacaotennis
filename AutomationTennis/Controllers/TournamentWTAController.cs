using AutomationTennis.Domain;
using AutomationTennis.DTO;
using AutomationTennis.Services.TournamentWTAService;
using Microsoft.AspNetCore.Mvc;

namespace AutomationTennis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentWTAController : ControllerBase
    {
        private readonly ILogger<TournamentWTAController> _logger;
        private readonly ITournamentWTAService _tournamentWTAService;

        public TournamentWTAController(ILogger<TournamentWTAController> logger,
            ITournamentWTAService tournamentWTAService)
        {
            _logger = logger;
            _tournamentWTAService = tournamentWTAService;
        }

        [HttpGet("GetAllTournamentWTA")]
        public async Task<ActionResult<ResponseModel<IEnumerable<TournamentWTA>>>> GetAllTournamentWTA()
        {
            var tournamentList = await _tournamentWTAService.GetAllTournamentWTA();
            return Ok(tournamentList);
        }

        [HttpGet("GetTournamentWTAById/{idTournament}")]
        public async Task<ActionResult<ResponseModel<TournamentWTA>>> GetTournamentWTAById(int idTournament)
        {
            var tournament = await _tournamentWTAService.GetTournamentWTAById(idTournament);
            return Ok(tournament);
        }

        [HttpPost("AddListTournamentOfMonthWTAFromGenericApi")]
        public async Task<ActionResult> AddListTournamentOfMonthWTAFromGenericApi()
        {
            await _tournamentWTAService.AddListTournamentOfMonthWTAFromGenericApi();
            return Ok();
        }

        [HttpPost("SendTournamentListOfMonthToSlackChannelWTA")]
        public async Task<ActionResult> SendTournamentListOfMonthToSlackChannelWTA()
        {
            await _tournamentWTAService.SendTournamentListOfMonthToSlackChannelWTA();
            return Ok();
        }

    }
}
