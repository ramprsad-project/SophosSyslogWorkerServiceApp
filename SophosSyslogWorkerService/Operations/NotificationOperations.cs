using Npgsql;
using SophosSyslogWorkerService.Common;
using SophosSyslogWorkerService.Mapping;
using SophosSyslogWorkerService.Models;
using System.Data;
using System.Net;
using System.Net.Mail;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace SophosSyslogWorkerService.Operations
{
    internal class NotificationOperations
    {
        public static List<User>? _users { get; set; }

        public IConfiguration? _configuration { get; set; }

        public NotificationOperations(IConfiguration? configuration)
        {
            _configuration = configuration;
        }
        public  void GetUserDetails(IConfiguration _configuration,NpgsqlConnection _dbcon)
        {
            string? connstring = _configuration.GetSection("ConnectionStrings").GetSection("SyslogDB_Windows").Value;
            DataSet dsSophos = new DataSet();
            DataTable dtSophos = new DataTable();
            try
            {
                NpgsqlDataAdapter dataAdapter = new NpgsqlDataAdapter(Commands.GetUserDetails, new NpgsqlConnection(connstring));
                // reset DataSet before i do
                dsSophos.Reset();
                // filling DataSet with result from NpgsqlDataAdapter
                dataAdapter.Fill(dsSophos);
                // since it C# DataSet can handle multiple tables, we will select first
                dtSophos = dsSophos.Tables[0];
                _users = ModelMapper.MapUserValues(dtSophos).ToList<User>();
            }
            catch { }
            finally { _dbcon.Close(); }
        }



        public  void SendNotification(Item item, EventAction eventAction)
        {
            string htmlString = "<html>\r\n<head>\r\n  <title>" + eventAction.Type + "</title>\r\n  <style>\r\n    body {\r\n      font-family: Arial, sans-serif;\r\n      line-height: 1.6;\r\n    }\r\n\r\n    h1 {\r\n      color: #333333;\r\n      font-size: 24px;\r\n      margin-bottom: 20px;\r\n    }\r\n\r\n    p {\r\n      margin-bottom: 10px;\r\n    }\r\n\r\n    .highlight {\r\n      background-color: #ffd700;\r\n      padding: 5px;\r\n      font-weight: bold;\r\n    }\r\n\r\n    .details {\r\n      margin-top: 20px;\r\n      padding: 10px;\r\n      background-color: #f5f5f5;\r\n      border: 1px solid #dddddd;\r\n    }\r\n  </style>\r\n</head>\r\n<body>\r\n  <h1>Policy Violation Notice</h1>\r\n  <p>Dear User,</p>\r\n  <p>We regret to inform you that you are trying to violating the policy.</p>\r\n  <div class=\"details\">\r\n    <p><span class=\"highlight\">Policy Category:</span> " + eventAction.Name + "</p>\r\n    <p><span class=\"highlight\">Violation Type:</span> " + eventAction.Type + ".</p>\r\n    <p>Please review our policies and guidelines to understand what changes need to be made to comply with our standards. Once you have rectified the issue, please notify us immediately so that we can reassess the situation.</p>\r\n    <p>If you have any questions or need further assistance, please don't hesitate to contact our support team at <h5>support@bigdogbusiness.com</h5></p>\r\n    <p>Sincerely,</p>\r\n    <p><h4>BigDog Business Team</h4></p>\r\n  </div>\r\n</body>\r\n</html>\r\n";
            foreach (User user in _users)
            {
                if (Convert.ToString(user.ID) == item.endpoint_id)
                {
                    if ((bool)eventAction.ByEmail)
                    {
                        SendEmail(htmlString, _configuration.GetSection("EmailAuthDetails").GetSection("Subject").Value, _configuration.GetSection("EmailAuthDetails").GetSection("From").Value, user.PrimaryEmail, _configuration.GetSection("EmailAuthDetails").GetSection("Password").Value);
                    }
                    if ((bool)eventAction.BySMS)
                    {
                        SendSMS(user.PrimaryMobile, "You have Violated the Web Policy on " + item.when, "BigDog Business");//_configuration.GetSection("SMSAuthDetails").GetSection("FromMobileNumber").Value
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
            var messageOptions = new CreateMessageOptions(new PhoneNumber(number));
            messageOptions.From = new PhoneNumber(_configuration.GetSection("SMSAuthDetails").GetSection("RegisteredMobileNumber").Value);
            messageOptions.Body = "This message is from" + sender + ":" + message;
            MessageResource _message = MessageResource.Create(messageOptions);
            return result;
        }
    }
}
