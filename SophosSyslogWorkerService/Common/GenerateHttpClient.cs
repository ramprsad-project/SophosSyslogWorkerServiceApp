using System.Net.Http.Headers;

namespace SophosSyslogWorkerService.Common
{
    internal class GenerateHttpClient
    {
        public HttpClient GetHttpClient(IConfiguration _configuration, string? _token)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(_configuration.GetSection("SophosCentralAPIURLS").GetSection("BaseURL").Value);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Add the Authorization header with the AccessToken.
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _token);
            return client;
        }
    }
}
