using System.Net;

using Microsoft.AspNetCore.Mvc;

using IRFestival.Api.Data;
using IRFestival.Api.Domain;
using IRFestival.Api.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.ApplicationInsights;

namespace IRFestival.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FestivalController : ControllerBase
    {
        private FestivalDbContext _ctx;
        private TelemetryClient _telemetryClient;

        public FestivalController(FestivalDbContext festivalContext, TelemetryClient telemetry)
        {
            _ctx = festivalContext;
            _telemetryClient = telemetry;
        }


        [HttpGet("LineUp")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Schedule))]
        public async Task<ActionResult> GetLineUp()
        {
                var lineUp = await _ctx.Schedules.Include(x => x.Festival)
                                                            .Include(x => x.Items)
                                                            .ThenInclude(x => x.Artist)
                                                            .Include(x => x.Items)
                                                            .ThenInclude(x => x.Stage)
                                                            .FirstOrDefaultAsync();

                return Ok(lineUp);
        }

        [HttpGet("Artists")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Artist>))]
        public async  Task<ActionResult> GetArtists(bool? withRatings)
        {
            if (withRatings.HasValue && withRatings.Value)
                _telemetryClient.TrackEvent($"List of artists with ratings");
            else
                _telemetryClient.TrackEvent($"List of artists with ratings");
            var artists = await _ctx.Artists.ToListAsync();
            return Ok(artists);
        }

        [HttpGet("Stages")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Stage>))]
        public async Task<ActionResult> GetStages()
        {
            var stages = await _ctx.Stages.ToListAsync();
            return Ok(stages);
        }

        [HttpPost("Favorite")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ScheduleItem))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> SetAsFavorite(int id)
        {
            var schedule = await _ctx.Schedules.Select(x => x.Items.FirstOrDefault(x => x.Id == id)).FirstAsync();

            if (schedule != null)
            {
                schedule.IsFavorite = !schedule.IsFavorite;
                return Ok(schedule);
            }
            return NotFound();
        }

    }
}