using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophosSyslogWorkerService.Models
{
    internal class User
    {
        public Guid? ID { get; set; }
        public string? Name { get; set; }
        public string? PrimaryEmail { get; set; }
        public string? SecondaryEmail { get; set; }
        public string? PrimaryMobile { get; set; }
        public string? SecondaryMobile { get; set; }
    }
}
