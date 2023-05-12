using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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

