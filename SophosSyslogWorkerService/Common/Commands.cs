using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophosSyslogWorkerService.Common
{
    internal class Commands
    {
        public static string? GetPolicyDetails = "SELECT id, policy_id, name, type, created_by, created_on, is_deleted, settings, owner_id FROM policy_details where is_deleted = false;";

        public static string? GetUserDetails = "SELECT user_id, user_name, user_email_primary, user_email_secondary, user_mobile_primary, user_mobile_secondary FROM user_details where user_status = true;";

        public static string? GetActionDetails = "SELECT event_class_name, event_type_name, event_action_name, event_action_by_mail, event_action_by_sms FROM event_action_details;";

        public static string? InsertSystemEvents = "INSERT INTO sophos_system_events(event_id, severity, name, location, type, created_at, source_info_ip, customer_id, endpoint_type, endpoint_id, user_id, when_occured, source, group_action) VALUES ('";

        public static string? GetLastRecordDateTime = "SELECT created_at FROM sophos_system_events ORDER BY id DESC LIMIT 1;";
    }
}
