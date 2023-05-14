using System.Text.Json.Serialization;

namespace VkWallPostgre.Dto
{
    public class VkToken
    {
        [JsonPropertyName("access_token")]
        public string Token { get; set; } = default!;

        [JsonPropertyName("expires_in")]
        public int Expired { get; set; }

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }
    }
}
