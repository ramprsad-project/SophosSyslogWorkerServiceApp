using SophosSyslogWorkerService.Models;
using System.Data;

namespace SophosSyslogWorkerService.Mapping
{
    internal class ModelMapper
    {
        public static IList<PolicyDetails> MapPolicyDetailValues(DataTable dtPolicyDetails)
        {
            IList<PolicyDetails> policyDetails = dtPolicyDetails.AsEnumerable().Select(row =>
                new PolicyDetails(row.Field<int>("id"), row.Field<Guid>("policy_id"), row.Field<string>("name"), row.Field<string>("type"), row.Field<string>("created_by"), row.Field<DateTime>("created_on"), row.Field<string>("settings"), row.Field<Guid>("owner_id"), row.Field<bool>("is_deleted"))).ToList();
            return policyDetails;
        }

        public static IList<User> MapUserValues(DataTable dtSophos)
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

        public static IList<EventAction> MapEventActionValues(DataTable dtSophos)
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
}
