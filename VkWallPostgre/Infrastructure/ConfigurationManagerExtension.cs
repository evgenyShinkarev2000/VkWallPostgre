namespace VkWallPostgre.Infostracture
{
    public static class ConfigurationManagerExtension
    {
        public static OpenIdParams GetOpenIdHelperParams(this ConfigurationManager configurationManager)
        {
            var section = configurationManager.GetRequiredSection("VkOpenIdParams");
            var clientdSecret = section.GetValue<string>("ClientSecret");
            if (clientdSecret == null)
            {
                throw new Exception("Can't find ClientSecret in config");
            }

            var clientId = section.GetValue<string>("ClientId");
            if (clientId == null)
            {
                throw new Exception("Can't find ClientId in config");
            }

            return new OpenIdParams() { ClientId = clientId, ClientSecret = clientdSecret };
        }

        public static string GetPostgreConnectionString(this ConfigurationManager configurationManager)
        {
            var connectionString = configurationManager.GetConnectionString("postgre");
            if (connectionString == null)
            {
                throw new Exception("Can't find postgre connection string in config");
            }

            return connectionString;
        }
    }
}
