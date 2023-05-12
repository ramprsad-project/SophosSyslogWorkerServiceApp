using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

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
