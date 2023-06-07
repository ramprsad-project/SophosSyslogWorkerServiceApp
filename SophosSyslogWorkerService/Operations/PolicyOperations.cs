using Npgsql;
using SophosSyslogWorkerService.Common;
using SophosSyslogWorkerService.Mapping;
using SophosSyslogWorkerService.Models;
using System.Data;

namespace SophosSyslogWorkerService.Operations
{
    internal class PolicyOperations
    {
        public static List<PolicyDetails> _policyDetails { get; set; }

        string? connstring;
        public static void GetPolicyDetails(NpgsqlConnection _dbcon)
        {
            DataSet dsPolicyDetails = new DataSet();
            DataTable dtPolicyDetails = new DataTable();
            try
            {
                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(Commands.GetPolicyDetails, _dbcon);
                // reset DataSet before i do
                dsPolicyDetails.Reset();
                // filling DataSet with result from NpgsqlDataAdapter
                dataAdapter.Fill(dsPolicyDetails);
                // since it C# DataSet can handle multiple tables, we will select first
                dtPolicyDetails = dsPolicyDetails.Tables[0];
                _policyDetails = ModelMapper.MapPolicyDetailValues(dtPolicyDetails).ToList<PolicyDetails>();
            }
            catch { }
            finally
            {
                _dbcon.Close();
            }
        }
        private static bool IsValidPolicy()
        {
            return true;
        }

        public static bool ApplyPolicyToUser(IConfiguration _configuration)
        {
           GetPolicyDetails(new NpgsqlConnection(_configuration.GetSection("ConnectionStrings").GetSection("SyslogDB_Windows").Value));

            if (IsValidPolicy())
            {
                foreach (PolicyDetails policy in _policyDetails)
                {
                    string uid = policy.settings;
                }
                return true;
            }
            return false;
        }
    }
}
