using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophosSyslogWorkerService.Common
{
    internal static  class DBConnection
    {
        static string _connstring = _configuration.GetSection("ConnectionStrings").GetSection("SyslogDB_Windows").Value;
        public static string DBConnectionString { get { return _connstring; } set { value = _connstring; } }
    }
}
