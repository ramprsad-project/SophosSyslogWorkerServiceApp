using Newtonsoft.Json;
using Npgsql;

namespace SophosSyslogWorkerService
{
    internal class DBLogging
    {
        public IConfiguration? _configuration { get; set; }
        public DBLogging(IConfiguration? configuration)
        {
            _configuration = configuration;
        }
        public string SaveSystemEventsToDB(string? event_id, string? severity, string? name, string? location, string? type, string? created_at, string? source_info_ip, string? customer_id, string? endpoint_type, string? endpoint_id, string? user_id, string? when_occured, string? source, string? group_action)
        {
            string? connstring = _configuration.GetSection("ConnectionStrings").GetSection("SyslogDB_Windows").Value;
            NpgsqlConnection dbcon = new NpgsqlConnection(connstring);
            dbcon.Open();
            NpgsqlCommand dbcmd = dbcon.CreateCommand();
            try
            {
                string sql1 = "INSERT INTO sophossystemevents(event_id, severity, name, location, type, created_at, source_info_ip, customer_id, endpoint_type, endpoint_id, user_id, when_occured, source, group_action) " +
                " VALUES ('" + event_id + "','" + severity + "','" + name + "','" + location + "','" + type + "','" + created_at + "','" + source_info_ip + "','" + customer_id + "','" + endpoint_type + "','" + endpoint_id + "','" + user_id + "','" + when_occured + "','" + source + "','" + group_action + "')";
                dbcmd.CommandText = sql1;
                dbcmd.ExecuteNonQuery();
                new LogsMonitor(_configuration, dbcon).MonitorSystemEvents(type, endpoint_id, when_occured);//user_id
                return "successfully inserted data.";
            }
            catch (NpgsqlException ex)
            {
                if (ex.Data == null)
                { return "failed to insert data."; }
                else
                { return "failed to insert data."; }
            }
            finally { dbcon.Close(); }
        }

        /// <summary>
        /// SaveSystemEventsToDB
        /// </summary>
        /// <returns></returns>
        public string ExecuteSaveSystemEventsToDB(string token)
        {
            string events = new SystemEvents().GetTenantEvents(_configuration, token, UrlOrganizer.GetUrl("SophosCentralAPIURLS", "TenantEvents", _configuration));
            string status = "";
            List<Item> items = Deserializer.Deserialize<EndPointSystemEvents>(events).items;
            foreach (Item item in items)
            {
                status = SaveSystemEventsToDB(item.id, item.severity, item.name, item.location, item.type, item.created_at, item.source_info.ip, item.customer_id, item.endpoint_type, item.endpoint_id, item.user_id, item.when, item.source, item.group);
            }
            return status;
        }

        public static class Deserializer
        {
            public static T Deserialize<T>(string json)
            {
                Newtonsoft.Json.JsonSerializer s = new JsonSerializer();
                return s.Deserialize<T>(new JsonTextReader(new StringReader(json)));
            }
        }
    }
}
