using Newtonsoft.Json.Linq;
using SophosSyslogWorkerService.Common;

namespace SophosSyslogWorkerService
{
    internal class PartnerDetails
    {
        public JObject? PartnerDetailsObject { get; set; }
        public IConfiguration _configuration { get; set; }
        public string? _token { get; set; }
        public string? _partnerAPIUrl { get; set; }

        public PartnerDetails(IConfiguration configuration, string? token, string? partnerAPIUrl)
        {
            _configuration = configuration;
            _token = token;
            _partnerAPIUrl = partnerAPIUrl;
        }

        public string GetPartnerID()
        {
            var client = new GenerateHttpClient().GetHttpClient(_configuration, _token);
            HttpResponseMessage response =  client.GetAsync(_partnerAPIUrl).Result;
            PartnerDetailsObject = JObject.Parse( response.Content.ReadAsStringAsync().Result);
            dynamic? data = PartnerDetailsObject;
            return data.id;
        }
    }
}

