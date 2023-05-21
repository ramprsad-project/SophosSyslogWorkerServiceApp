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

        public void MonitorSystemEvents(Item item)
        {
            GetActionDetails();
            foreach (EventAction eventAction in _eventAction)
            {
                GetUserDetails();
                
                if (eventAction.Type == item.type)
                {
                    SendNotification(item, eventAction);
                }
            }
        }

        public void SendNotification(Item item, EventAction eventAction)
        {
            string htmlString = "<html>\r\n<head>\r\n  <title>Website Policy Violation Notice</title>\r\n  <style>\r\n    body {\r\n      font-family: Arial, sans-serif;\r\n      line-height: 1.6;\r\n    }\r\n\r\n    h1 {\r\n      color: #333333;\r\n      font-size: 24px;\r\n      margin-bottom: 20px;\r\n    }\r\n\r\n    p {\r\n      margin-bottom: 10px;\r\n    }\r\n\r\n    .highlight {\r\n      background-color: #ffd700;\r\n      padding: 5px;\r\n      font-weight: bold;\r\n    }\r\n\r\n    .details {\r\n      margin-top: 20px;\r\n      padding: 10px;\r\n      background-color: #f5f5f5;\r\n      border: 1px solid #dddddd;\r\n    }\r\n  </style>\r\n</head>\r\n<body>\r\n  <h1>Website Policy Violation Notice</h1>\r\n  <p>Dear User,</p>\r\n  <p>We regret to inform you that you are trying to access the website which was blocked due to a policy violation.</p>\r\n  <div class=\"details\">\r\n    <p><span class=\"highlight\">Website URL:</span> www.example.com</p>\r\n    <p><span class=\"highlight\">Violation Type:</span> "+ eventAction.Type +".</p>\r\n    <p>Please review our policies and guidelines to understand what changes need to be made to comply with our standards. Once you have rectified the issue, please notify us immediately so that we can reassess the situation.</p>\r\n    <p>If you have any questions or need further assistance, please don't hesitate to contact our support team at <h5>support@bigdogbusiness.com</h5></p>\r\n    <p>Sincerely,</p>\r\n    <p><h4>BigDog Business Team</h4></p>\r\n  </div>\r\n</body>\r\n</html>\r\n";
            foreach (User user in _users)
            {
                if (Convert.ToString(user.ID) == item.endpoint_id)
                {
                    if ((bool)eventAction.ByEmail)
                    {
                        SendEmail(htmlString, _configuration.GetSection("EmailAuthDetails").GetSection("Subject").Value, _configuration.GetSection("EmailAuthDetails").GetSection("From").Value, _configuration.GetSection("EmailAuthDetails").GetSection("To").Value, _configuration.GetSection("EmailAuthDetails").GetSection("Password").Value);
                    }
                    if ((bool)eventAction.BySMS)
                    {
                        SendSMS(_configuration.GetSection("SMSAuthDetails").GetSection("FromMobileNumber").Value, "You are Violate the Web Policy on " + item.when, "BigDog Business");
                    }
                }
            }
        }

        public void SendEmail(string? htmlString, string? subject, string? from, string? to, string? fromPwd)
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

        public string SendSMS(string? number, string? message, string? sender)
        {
            string result = String.Empty;
            var accountSid = _configuration.GetSection("SMSAuthDetails").GetSection("AccountSID").Value;
            var authToken = _configuration.GetSection("SMSAuthDetails").GetSection("AuthToken").Value;
            TwilioClient.Init(accountSid, authToken);

            var messageOptions = new CreateMessageOptions(
              new PhoneNumber(number));
            messageOptions.From = new PhoneNumber(_configuration.GetSection("SMSAuthDetails").GetSection("RegisteredMobileNumber").Value);
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