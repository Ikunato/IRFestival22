using IRFestival.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Net;

namespace IRFestival.Api.Controllers
{
    public class ArticlesController : ControllerBase
    {
        private CosmosClient _cosmos;
        private Container _websiteArticlesContainer;

        public ArticlesController(IConfiguration config)
        {
            var task = Task.Run(async () =>
            {
                _cosmos = new(config.GetConnectionString("CosmosConnection"));
                _websiteArticlesContainer = _cosmos.GetContainer("IRFestivalArticles", "WebsiteArticles");
            });
            task.Wait();
        }

        [HttpPost("Article")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Article))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> CreateArticle(Article dummy)
        {
            if (dummy == null) return NoContent();
            var result = await _websiteArticlesContainer.CreateItemAsync(dummy);

            return Ok(dummy);
        }

        [HttpGet("Article")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Article))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult> GetPublishedArticles()
        {
            List<Article> result = new();
            var queryDefinition = _websiteArticlesContainer.GetItemLinqQueryable<Article>()
                .Where(p => p.Status == nameof(Status.Published))
                .OrderBy(p => p.Date);

            var iterator = queryDefinition.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                result = response.ToList(); 
            }

            return Ok(result);
        }
    }
}
