namespace SophosSyslogWorkerService
{
    internal static class UrlOrganizer
    {
        public static string GetUrl(string category,string subCategory,IConfiguration _config)
        {
            return _config.GetSection(category).GetSection(subCategory).Value;
        }
    }
}
