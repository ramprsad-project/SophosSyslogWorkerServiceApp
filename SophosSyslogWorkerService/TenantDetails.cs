using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace SophosSyslogWorkerService
{
    internal class TenantDetails
    {
        public JObject? TenantDetailsObject { get; set; }
        public IConfiguration _configuration { get; set; }
        public string? _token { get; set; }
        public string? _tenantAPIUrl { get; set; }

        public TenantDetails(IConfiguration configuration, string? token, string? tenantAPIUrl)
        {
            _configuration = configuration;
            _token = token;
            _tenantAPIUrl = tenantAPIUrl;
            GetTenantDetails();
        }

        public void GetTenantDetails()
        {
            var client = new GenerateHttpClient().GetHttpClient(_configuration, _token);
            client.DefaultRequestHeaders.Add("X-Partner-ID",  new PartnerDetails(_configuration, _token, UrlOrganizer.GetUrl("SophosCentralAPIURLS", "PartnerUrl", _configuration)).GetPartnerID());
            HttpResponseMessage response =  client.GetAsync(_tenantAPIUrl).Result;
            TenantDetailsObject = JObject.Parse( response.Content.ReadAsStringAsync().Result);
        }
        public JObject GetTenants()
        {
            JObject? data = TenantDetailsObject;
            return data;
        }

        public string GetTenantID()
        {
            string? id = TenantDetailsObject.SelectToken("items").FirstOrDefault().SelectToken("id").ToString();
            return id;
        }

        public string GetTenantOrganizationID()
        {
            string? organizationid = TenantDetailsObject.SelectToken("items").FirstOrDefault().SelectToken("organization").FirstOrDefault().SelectToken("id").ToString();
            return organizationid;
        }

        public string GetAPIHostURL()
        {
            string? apiHost = TenantDetailsObject.SelectToken("items").FirstOrDefault().SelectToken("apiHost").ToString();
            return apiHost;
        }

        public string GetTenantStatus()
        {
            string? status = TenantDetailsObject.SelectToken("items").FirstOrDefault().SelectToken("status").ToString();
            return status;
        }

        public string GetDataRegion()
        {
            string? dataRegion = TenantDetailsObject.SelectToken("items").FirstOrDefault().SelectToken("dataRegion").ToString();
            return dataRegion;
        }

        public string GetTenantName()
        {
            string? name = TenantDetailsObject.SelectToken("items").FirstOrDefault().SelectToken("name").ToString();
            return name;
        }
    }
}

