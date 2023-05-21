using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophosSyslogWorkerService
{
    internal class EventLog
    {
        public string? event_id { get; set; }
        public string? severity { get; set; }
        public string? name { get; set; }
        public string? location { get; set; }
        public string? type { get; set; }
        public string? created_at { get; set; }
        public string? source_info_ip { get; set; }
        public string? customer_id { get; set; }
        public string? endpoint_type { get; set; }
        public string? endpoint_id { get; set; }
        public string? user_id { get; set; }
        public string? when_occured { get; set; }
        public string? source { get; set; }
        public string? group_action { get; set; }
    }
}
