using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophosSyslogWorkerService.Models
{
    internal class PolicyDetails
    {
        public int id { get; set; }
        public Guid policy_id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string created_by { get; set; }
        public DateTime created_on { get; set; }
        public string settings { get; set; }
        public Guid owner_id { get; set; }
        public bool is_deleted { get; set; }

        public PolicyDetails(int id_, Guid policy_id_, string name_, string type_, string created_by_, DateTime created_on_, string settings_, Guid owner_id_, bool is_deleted_)
        {
            this.id = id_;
            this.policy_id = policy_id_;
            this.name = name_;
            this.type = type_;
            this.created_by = created_by_;
            this.created_on = created_on_;
            this.settings = settings_;
            this.owner_id = owner_id_;
            this.is_deleted = is_deleted_;
        }
    }
}
