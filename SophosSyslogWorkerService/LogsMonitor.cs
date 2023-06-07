using Npgsql;
using SophosSyslogWorkerService.Common;
using SophosSyslogWorkerService.Models;
using SophosSyslogWorkerService.Operations;

namespace SophosSyslogWorkerService
{
    internal class LogsMonitor
    {
        public IConfiguration? _configuration { get; set; }
        public NpgsqlConnection? _dbcon { get; set; }

        /// <summary>
        /// LogsMonitor
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="dbcon"></param>
        public LogsMonitor(IConfiguration? configuration, NpgsqlConnection dbcon)
        {
            _configuration = configuration;
            _dbcon = dbcon;
        }

        /// <summary>
        /// MonitorSystemEvents
        /// </summary>
        /// <param name="item"></param>
        /// <param name="_eventAction"></param>
        public void MonitorSystemEvents(Item item)
        {
            bool IsNewEndpoint = false;
            NotificationOperations notificationOperations = new NotificationOperations(_configuration);

            if (IsNewEndpoint)
            {
                bool isApplied = PolicyOperations.ApplyPolicyToUser(_configuration);
            }
            List<EventAction> _eventAction = DBLogging.GetActionDetails(_configuration, _dbcon);
            foreach (EventAction eventAction in _eventAction)
            {
                notificationOperations.GetUserDetails(_configuration, _dbcon);
                if (eventAction.Type == item.type)
                {
                    notificationOperations.SendNotification(item, eventAction);
                }
            }
        }
    }
}