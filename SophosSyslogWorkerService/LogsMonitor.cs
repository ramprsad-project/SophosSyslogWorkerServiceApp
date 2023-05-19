using Npgsql;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace SophosSyslogWorkerService
{
    internal class LogsMonitor
    {
        public List<User>? _users { get; set; }
        public List<EventAction>? _eventAction { get; set; }
        public IConfiguration? _configuration { get; set; }
        public NpgsqlConnection? _dbcon { get; set; }

        public LogsMonitor(IConfiguration? configuration, NpgsqlConnection dbcon)
        {
            _configuration = configuration;
            _dbcon = dbcon;
        }

        public void MonitorSystemEvents(string? eventType, string? userId, string? when)
        {
            GetActionDetails();
            foreach (EventAction eventAction in _eventAction)
            {
                GetUserDetails();
                
                if (eventAction.Type == eventType)
                {
                    SendNotification(userId, when, eventAction);
                }
            }
        }

        public void SendNotification(string? userId, string? when, EventAction eventAction)
        {
            string? phoneNumber = null;
            foreach (User user in _users)
            {
                //if (user.ID == userId)
                //{
                if ((bool)eventAction.ByEmail)
                {
                    SendEmail("<h1>BigDog Business Inc.</h1> \n You are trying to Violate the Web Policy on " + when, "Information Message from BigDog Business.", "analyst.dbrp@gmail.com", "ram.prasad@navasoftware.com", "idujwwmmqfxzgzsm");
                }
                if ((bool)eventAction.BySMS)
                {
                    SendSMS("+919591435112", "You are trying to Violate the Web Policy on " + when, "BigDog Business");
                }
                //}
            }
        }

        public void SendEmail(string htmlString, string subject, string from, string to, string fromPwd)
        {
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(from);
                    mail.To.Add(to);
                    mail.Subject = subject;
                    mail.Body = htmlString;
                    mail.IsBodyHtml = true;
                    // mail.Attachments.Add(new Attachment("C:\\file.zip"));

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.Credentials = new NetworkCredential(from, fromPwd);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }
                }
            }
            catch (Exception) { }
        }

        public string SendSMS(string number, string message, string sender)
        {
            string result = String.Empty;
            var accountSid = "ACe32ee6f225828c8a93c6e3fb14444c78";
            var authToken = "c67d42a6c3188d8b9829eef525085d95";
            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(
              new PhoneNumber(number));
            messageOptions.From = new PhoneNumber("+12545365213");
            messageOptions.Body = "This message is from" + sender + ":" + message;


            MessageResource _message = MessageResource.Create(messageOptions);
            return result;
        }

        public void GetUserDetails()
        {
            string? connstring = _configuration.GetSection("ConnectionStrings").GetSection("SyslogDB_Windows").Value;
            string commandText = "SELECT user_id, user_name, user_email_primary, user_email_secondary, user_mobile_primary, user_mobile_secondary FROM user_details where user_status = true;";
            DataSet dsSophos = new DataSet();
            DataTable dtSophos = new DataTable();
            try
            {
                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(commandText, new NpgsqlConnection(connstring));
                // reset DataSet before i do
                dsSophos.Reset();

                // filling DataSet with result from NpgsqlDataAdapter
                dataAdapter.Fill(dsSophos);

                // since it C# DataSet can handle multiple tables, we will select first
                dtSophos = dsSophos.Tables[0];
                _users = MapUserValues(dtSophos).ToList<User>();
            }
            catch { }
            finally { _dbcon.Close(); }
        }

        public void GetActionDetails()
        {
            string? connstring = _configuration.GetSection("ConnectionStrings").GetSection("SyslogDB_Windows").Value;
            string commandText = "SELECT event_class_name, event_type_name, event_action_name, event_action_by_mail, event_action_by_sms FROM event_action_details;";
            DataSet dsSophos = new DataSet();
            DataTable dtSophos = new DataTable();
            try
            {
                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(commandText, new NpgsqlConnection(connstring));
                // reset DataSet before i do
                dsSophos.Reset();

                // filling DataSet with result from NpgsqlDataAdapter
                dataAdapter.Fill(dsSophos);

                // since it C# DataSet can handle multiple tables, we will select first
                dtSophos = dsSophos.Tables[0];

                _eventAction = MapEventActionValues(dtSophos).ToList<EventAction>();
            }
            catch { }
            finally { _dbcon.Close(); }
        }

        private IList<User> MapUserValues(DataTable dtSophos)
        {
            IList<User> users = dtSophos.AsEnumerable().Select(row =>
                new User
                {
                    ID = row.Field<Guid>("user_id"),
                    Name = row.Field<string>("user_name"),
                    PrimaryEmail = row.Field<string>("user_email_primary"),
                    PrimaryMobile = row.Field<string>("user_mobile_primary"),
                    SecondaryEmail = row.Field<string>("user_email_secondary"),
                    SecondaryMobile = row.Field<string>("user_mobile_secondary"),
                }).ToList();
            return users;
        }

        private IList<EventAction> MapEventActionValues(DataTable dtSophos)
        {
            IList<EventAction> eventAction = dtSophos.AsEnumerable().Select(row =>
                 new EventAction
                 {
                     Name = row.Field<string>("event_class_name"),
                     Type = row.Field<string>("event_type_name"),
                     Action = row.Field<string>("event_action_name"),
                     ByEmail = row.Field<bool>("event_action_by_mail"),
                     BySMS = row.Field<bool>("event_action_by_sms"),

                 }).ToList();
            return eventAction;
        }
    }

    internal class User
    {
        public Guid? ID { get; set; }
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
        public bool? ByEmail { get; set; }
        public bool? BySMS { get; set; }
    }
}