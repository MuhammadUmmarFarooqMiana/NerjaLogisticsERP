namespace NerjaLogisticsERP.Domain.Enums;

public enum AccountStatus
{
    Incomplete,        // registered, profile not yet submitted

    PendingReview,   // profile submitted, awaiting Admin/Supervisor decision

    Active,

    Suspended,

    Rejected,          // needs correction — can be edited and resubmitted

    Terminated,
}
