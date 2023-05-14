namespace VkWallPostgre.Infostracture
{
    public class OpenIdParams
    {
        public string ClientId { get; set; } = default!;
        public string ClientSecret { get; set; } = default!;
        public string RedirectUriBase { get; set; } = default!;
    }
}
