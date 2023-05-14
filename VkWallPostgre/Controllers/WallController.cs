using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using VkWallPostgre.Data;
using VkWallPostgre.Dto;
using VkWallPostgre.Infostracture;

namespace VkWallPostgre.Controllers
{
    [Route("api/Wall")]
    public class WallController : Controller
    {
        private readonly IDistributedCache cache;
        private readonly AppDbContext appDbContext;
        private readonly ISimpleLogger logger;

        public WallController(IDistributedCache cache, AppDbContext appDbContext, ISimpleLogger logger)
        {
            this.cache = cache;
            this.appDbContext = appDbContext;
            this.logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain">
        /// Used id from token, when not specifed or not existed
        /// </param>
        /// <returns></returns>

        [HttpGet("Posts")]
        public async Task<IActionResult> GetPosts([FromQuery] string domain = "")
        {
            var result = await TryGetPosts(domain);
            if (result.error != null)
            {
                return result.error;
            }

            return Ok(result.posts!);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="domain">
        /// Used id from token, when not specifed or not existed
        /// </param>
        /// <returns></returns>
        [HttpPost("ProcessPosts")]
        public async Task<IActionResult> ProcessPosts([FromQuery] string domain = "")
        {
            var result = await TryGetPosts(domain);
            if (result.error != null)
            {
                return result.error;
            }
            var processed = ProcessPosts(result.posts!);
            var processedPost = new ProcesedPost() { Domain = domain, Tokens = processed };
            appDbContext.ProcesedPosts.Add(processedPost);

            appDbContext.SaveChanges();

            return Ok(processedPost);
        }

        [HttpGet("ProcessedPosts")]
        public IActionResult GetAllProcesed()
        {
            return Ok(appDbContext.ProcesedPosts.ToArray());
        }

        private async Task<(IEnumerable<string>? posts, IActionResult? error)> TryGetPosts(string domain)
        {
            var token = cache.GetVkToken();
            if (token == null)
            {
                return (null, Redirect("~/api/Auth/CodeFlow"));
            }

            var response = await new HttpClient().GetFromJsonAsync<WallGetResponse>(BuildPostsRequestString(domain, token));
            if (response == null)
            {
                return (null, StatusCode(500, "Can't get data from remote server"));
            }
            var items = response?.Response?.Items;
            if (items == null)
            {
                return (null, StatusCode(500, "response doesn't contain items"));
            }

            var posts = items.Select(i => i.Text) as IEnumerable<string>;
            if (posts == null)
            {
                return (null, StatusCode(500, "items doesn't contain text"));
            }

            return (posts, null);
        }

        private string BuildPostsRequestString(string domain, string token)
        {
            var queries = new KeyValuePair<string, string?>[]
            {
                new KeyValuePair<string, string?>("access_token", token),
                new KeyValuePair<string, string?>("extended", "0"),
                new KeyValuePair<string, string?>("v", "5.131"),
                new KeyValuePair<string, string?>("domain", domain),
                new KeyValuePair<string, string?>("limit", "5"),
            };

            var queriesString = QueryString.Create(queries);

            return $"https://api.vk.com/method/wall.get{queriesString}";
        }

        private string ProcessPosts(IEnumerable<string> posts)
        {
            logger.Log("begin process posts");
            var dictionary = new Dictionary<char, int>();

            foreach (var post in posts)
            {
                foreach (var symbol in post)
                {
                    if (Char.IsLetter(symbol))
                    {
                        var lowerSymbol = Char.ToLower(symbol);
                        if (dictionary.ContainsKey(lowerSymbol))
                        {
                            dictionary[lowerSymbol] += 1;
                        }
                        else
                        {
                            dictionary.Add(lowerSymbol, 1);
                        }
                    }
                }
            }

            var result = string.Join(" ", dictionary
                .OrderBy(p => p.Key)
                .Select(pair => $"{pair.Key}{pair.Value}"));

            logger.Log("end process posts");

            return result;
        }
    }
}
