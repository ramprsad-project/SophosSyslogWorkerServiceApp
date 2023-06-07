using SophosSyslogWorkerService.Common;

namespace SophosSyslogWorkerService
{
    internal class SystemEvents
    {
        /// <summary>
        /// GetTenantEvents
        /// </summary>
        /// <returns></returns>
        public string GetTenantEvents(IConfiguration _configuration, string? _token, string? _endpointEventsAPIUrl,string tenantId)
        {
            var client = new GenerateHttpClient().GetHttpClient(_configuration, _token);
           // string Id = new TenantDetails(_configuration, _token, UrlOrganizer.GetUrl("SophosCentralAPIURLS", "TenantUrl", _configuration)).GetTenantID();
            client.DefaultRequestHeaders.Add("X-Tenant-ID", tenantId);
            HttpResponseMessage response = client.GetAsync(_endpointEventsAPIUrl).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
