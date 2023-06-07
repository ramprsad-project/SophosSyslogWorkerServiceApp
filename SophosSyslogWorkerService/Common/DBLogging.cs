using Newtonsoft.Json;
using Npgsql;
using SophosSyslogWorkerService.Mapping;
using SophosSyslogWorkerService.Models;
using System.Data;

namespace SophosSyslogWorkerService.Common
{
    internal class DBLogging
    {
        public IConfiguration? _configuration { get; set; }
        public static List<EventAction>? _eventAction { get; set; }

        public DBLogging(IConfiguration? configuration)
        {
            _configuration = configuration;
        }
        public static List<EventAction> GetActionDetails(IConfiguration _configuration, NpgsqlConnection _dbcon)
        {
            string? connstring = _configuration.GetSection("ConnectionStrings").GetSection("SyslogDB_Windows").Value;
            DataSet dsSophos = new DataSet();
            DataTable dtSophos = new DataTable();
            try
            {
                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(Commands.GetActionDetails, new NpgsqlConnection(connstring));
                // reset DataSet before i do
                dsSophos.Reset();
                // filling DataSet with result from NpgsqlDataAdapter
                dataAdapter.Fill(dsSophos);
                // since it C# DataSet can handle multiple tables, we will select first
                dtSophos = dsSophos.Tables[0];
                _eventAction = ModelMapper.MapEventActionValues(dtSophos).ToList<EventAction>();
            }
            catch { }
            finally { _dbcon.Close(); }
            return _eventAction;
        }
        public string SaveSystemEventsToDB(Item item)
        {
            string? connstring = _configuration.GetSection("ConnectionStrings").GetSection("SyslogDB_Windows").Value;
            string? lastinsert = getLastLogDateTime();

            NpgsqlConnection dbcon = new NpgsqlConnection(connstring);
            dbcon.Open();
            NpgsqlCommand dbcmd = dbcon.CreateCommand();
            try
            {
                //if (string.IsNullOrEmpty(lastinsert) || (DateTime.Parse(lastinsert) < DateTime.Parse(item.created_at)))
                //{
                    string sql1 = Commands.InsertSystemEvents + item.id + "','" + item.severity + "','" + item.name.Replace("'","") + "','" + item.location + "','" + item.type + "','" + item.created_at + "','" + item.source_info.ip + "','" + item.customer_id + "','" + item.endpoint_type + "','" + item.endpoint_id + "','" + item.user_id + "','" + item.when + "','" + item.source + "','" + item.group + "')";
                    dbcmd.CommandText = sql1;
                    dbcmd.ExecuteNonQuery();
                    new LogsMonitor(_configuration, dbcon).MonitorSystemEvents(item);
                //}

                return "successfully read data.";
            }
            catch (NpgsqlException ex)
            {
                if (ex.Data == null)
                { return "failed to read data."; }
                else
                { return "failed to read data."; }
            }
            finally { dbcon.Close(); }
        }
        private string getLastLogDateTime()
        {
            string? connstring = _configuration.GetSection("ConnectionStrings").GetSection("SyslogDB_Windows").Value;
            DataSet dsSophos = new DataSet();
            DataTable dtSophos = new DataTable();
            try
            {
                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(Commands.GetLastRecordDateTime, new NpgsqlConnection(connstring));
                // reset DataSet before i do
                dsSophos.Reset();
                // filling DataSet with result from NpgsqlDataAdapter
                dataAdapter.Fill(dsSophos);
                // since it C# DataSet can handle multiple tables, we will select first
                return dsSophos.Tables[0].Rows[0].ItemArray[0].ToString();
            }
            catch (NpgsqlException ex)
            {
                if (ex.Data == null)
                { return "failed to insert data."; }
                else
                { return "failed to insert data."; }
            }
            finally {  }
        }
        /// <summary>
        /// SaveSystemEventsToDB
        /// </summary>
        /// <returns></returns>
        public string ExecuteSaveSystemEventsToDB(string token)
        {
            List<string> tenants = new TenantDetails(_configuration, token, UrlOrganizer.GetUrl("SophosCentralAPIURLS", "TenantUrl", _configuration)).GetTenants();
            string status = "";
            foreach (string tenant in tenants)
            {
                string events = new SystemEvents().GetTenantEvents(_configuration, token, UrlOrganizer.GetUrl("SophosCentralAPIURLS", "TenantEvents", _configuration), tenant);
               
                List<Item> items = Deserializer.Deserialize<EndPointSystemEvents>(events).items;
                foreach (Item item in items)
                {
                    status = SaveSystemEventsToDB(item);
                }
            }
            return status;
        }

        public static class Deserializer
        {
            public static T Deserialize<T>(string json)
            {
                JsonSerializer s = new JsonSerializer();
                return s.Deserialize<T>(new JsonTextReader(new StringReader(json)));
            }
        }
    }
}
