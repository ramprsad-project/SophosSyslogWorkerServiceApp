using Npgsql;

namespace SophosSyslogWorkerService
{
    internal class DBCRUDOperations
    {
        public NpgsqlDataReader Read(string command,IConfiguration? _configuration)
        {
            
            string? connstring = _configuration.GetSection("ConnectionStrings").GetSection("SyslogDB_Windows").Value;
            NpgsqlConnection dbcon = new NpgsqlConnection(connstring);
            dbcon.Open();
            NpgsqlCommand dbcmd = dbcon.CreateCommand();
            try
            {
                dbcmd.CommandText = command;
                return dbcmd.ExecuteReader();
            }
            catch (NpgsqlException ex)
            {
                throw;
            }
            finally { dbcon.Close(); }
        }
    }
}
