using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophosSyslogWorkerService
{
    internal class LogsMonitor
    {
        public static List<User>? _users { get; set; }
        public static List<EventAction>? _eventAction { get; set; }
        public IConfiguration? _configuration { get; set; }
        public LogsMonitor(IConfiguration? configuration)
        {
            _configuration = configuration;
        }

        public static void MonitorSystemEvents(string? eventType, string? userId, string? when)
        {
            foreach (EventAction eventAction in _eventAction)
            {
                if (eventAction.Type == eventType)
                {
                    SendNotification(userId, when);
                }
            }
        }

        public static void SendNotification(string? userId, string? when)
        {
            string? phoneNumber = null;
            //user_name, user_email_primary, user_email_secondary, user_mobile_primary, user_mobile_secondary,
            foreach (User user in _users)
            {
                if (user.ID == userId)
                {

                }
            }
        }

        public void GetUserDetails()
        {
            string? connstring = _configuration.GetSection("ConnectionStrings").GetSection("SyslogDB_Windows").Value;
            string commandText = "SELECT user_id, user_name, user_email_primary, user_email_secondary, user_mobile_primary, user_mobile_secondary FROM public.user_details where user_status = true;";
            NpgsqlConnection dbcon = new NpgsqlConnection(connstring);
            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, dbcon))
            {
                using (NpgsqlDataReader reader = cmd.ExecuteReaderAsync().Result)
                    while (reader.ReadAsync().Result)
                    {
                        _users.Add(MapUserValues(reader));
                    }
            }
        }

        public void GetActionDetails()
        {
            string? connstring = _configuration.GetSection("ConnectionStrings").GetSection("SyslogDB_Windows").Value;
            string commandText = "SELECT event_class_name, event_type_name, event_action_name, event_action_by_mail, event_action_by_sms FROM public.event_action_details;";
            NpgsqlConnection dbcon = new NpgsqlConnection(connstring);
            using (NpgsqlCommand cmd = new NpgsqlCommand(commandText, dbcon))
            {
                using (NpgsqlDataReader reader = cmd.ExecuteReaderAsync().Result)
                    while (reader.ReadAsync().Result)
                    {
                        _eventAction.Add(MapEventActionValues(reader));
                    }
            }
        }

        private User MapUserValues(NpgsqlDataReader reader)
        {
            string? id = reader["user_id"] as string;
            string? name = reader["user_name"] as string;
            string? email1 = reader["user_email_primary"] as string;
            string? email2 = reader["user_email_secondary"] as string;
            string? mobile1 = reader["user_mobile_primary"] as string;
            string? mobile2 = reader["user_mobile_secondary"] as string;

            User user = new User()
            {
                ID = id,
                Name = name,
                PrimaryEmail = email1,
                SecondaryEmail = email2,
                PrimaryMobile = mobile1,
                SecondaryMobile = mobile2
            };

            return user;
        }


        private EventAction MapEventActionValues(NpgsqlDataReader reader)
        {
            string? name = reader["event_class_name"] as string;
            string? type = reader["event_type_name"] as string;
            string? action = reader["event_action_name"] as string;
            bool? byMail = reader["event_action_by_mail"] as bool?;
            bool? bySMS = reader["event_action_by_sms"] as bool?;

            EventAction eventAction = new EventAction()
            {
                Name = name,
                Type = type,
                Action = action,
                ByEmail = Convert.ToString(byMail),
                BySMS = Convert.ToString(bySMS)
            };

            return eventAction;
        }
    }

    internal class User
    {
        public string? ID { get; set; }
        public string? Name { get; set; }
        public string? PrimaryEmail { get; set; }
        public string? SecondaryEmail { get; set; }
        public string? PrimaryMobile { get; set; }
        public string? SecondaryMobile { get; set; }
    }

    internal class EventAction
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Action { get; set; }
        public string? ByEmail { get; set; }
        public string? BySMS { get; set; }
    }
}