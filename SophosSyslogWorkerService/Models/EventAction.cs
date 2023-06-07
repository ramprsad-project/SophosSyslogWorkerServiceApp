using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophosSyslogWorkerService.Models
{
    internal class EventAction
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Action { get; set; }
        public bool? ByEmail { get; set; }
        public bool? BySMS { get; set; }
    }
}
