using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace SophosSyslogWorkerService.Common
{
    internal class BearerToken
    {
        public string GetBearerToken(IConfiguration _configuration)
        {
            using (var client = new HttpClient())
            {
                string url = UrlOrganizer.GetUrl("SophosCentralAPIURLS", "BaseURL", _configuration);
                string ClientID = _configuration.GetSection("SophosAuthDetails").GetSection("ClientID").Value;
                string ClientSecret = _configuration.GetSection("SophosAuthDetails").GetSection("ClientSecret").Value;

                client.BaseAddress = new Uri(url);

                // We want the response to be JSON.
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "*/*");

                // Build up the data to POST.
                List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>();
                postData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                postData.Add(new KeyValuePair<string, string>("client_id", ClientID));
                postData.Add(new KeyValuePair<string, string>("client_secret", ClientSecret));
                postData.Add(new KeyValuePair<string, string>("scope", "token"));

                FormUrlEncodedContent content = new FormUrlEncodedContent(postData);

                // Post to the Server and parse the response.
                HttpResponseMessage response = client.PostAsync(url, content).Result;
                string jsonString = response.Content.ReadAsStringAsync().Result;
                object? responseData = JsonConvert.DeserializeObject(jsonString);

                // return the Access Token.
                return ((dynamic)responseData).access_token;
            }
        }
    }
}
