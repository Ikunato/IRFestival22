using System.Net;

using Microsoft.AspNetCore.Mvc;

using IRFestival.Api.Data;
using IRFestival.Api.Domain;
using IRFestival.Api.Context;
using Microsoft.EntityFrameworkCore;

namespace IRFestival.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FestivalController : ControllerBase
    {
        private FestivalDbContext _ctx;

        public FestivalController(FestivalDbContext festivalContext)
        {
            _ctx = festivalContext;
        }


        [HttpGet("LineUp")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Schedule))]
        public async Task<ActionResult> GetLineUp()
        {
                List<Schedule> lineUp = await _ctx.Schedules.Include(x => x.Festival)
                                                            .Include(x => x.Items)
                                                            .ThenInclude(x => x.Artist)
                                                            .Include(x => x.Items)
                                                            .ThenInclude(x => x.Stage)
                                                            .ToListAsync();

                return Ok(lineUp);
        }

        [HttpGet("Artists")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<Artist>))]
        public async  Task<ActionResult> GetArtists()
        {
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