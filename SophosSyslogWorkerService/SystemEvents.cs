using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace SophosSyslogWorkerService
{
    internal class SystemEvents
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GetTenantEvents(IConfiguration _configuration, string? _token, string? _endpointEventsAPIUrl)
        {
            var client = new GenerateHttpClient().GetHttpClient(_configuration, _token);
            string Id = new TenantDetails(_configuration, _token, UrlOrganizer.GetUrl("SophosCentralAPIURLS", "TenantUrl", _configuration)).GetTenantID();
            client.DefaultRequestHeaders.Add("X-Tenant-ID", Id);
            HttpResponseMessage response = client.GetAsync(_endpointEventsAPIUrl).Result;
            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
