using Microsoft.Extensions.Configuration;
using Npgsql;
using SophosSyslogWorkerService.Common;
using SophosSyslogWorkerService.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SophosSyslogWorkerService.Mapping;

namespace SophosSyslogWorkerService.Operations
{
    internal class PolicyOperations
    {
        public static List<PolicyDetails> GetPolicyDetails(IConfiguration _configuration)
        {
            string? connstring = _configuration.GetSection("ConnectionStrings").GetSection("SyslogDB_Windows").Value;
            DataSet dsPolicyDetails = new DataSet();
            DataTable dtPolicyDetails = new DataTable();
            try
            {
                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(Commands.GetPolicyDetails, new NpgsqlConnection(connstring));
                // reset DataSet before i do
                dsPolicyDetails.Reset();

                // filling DataSet with result from NpgsqlDataAdapter
                dataAdapter.Fill(dsPolicyDetails);

                // since it C# DataSet can handle multiple tables, we will select first
                dtPolicyDetails = dsPolicyDetails.Tables[0];
                return ModelMapper.MapPolicyDetailValues(dtPolicyDetails).ToList<PolicyDetails>();
            }
            catch { }
            finally { _dbcon.Close(); }
        }
        private bool IsValidPolicy()
        {
            return true;
        }

        public bool ApplyPolicyToUser(IConfiguration _configuration)
        {
            GetPolicyDetails(_configuration);

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
