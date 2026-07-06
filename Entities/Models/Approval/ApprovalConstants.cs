namespace Entities.Models.Approval
{
    /// <summary>
    /// Konstanta status dan tipe untuk modul Approval.
    /// </summary>
    public static class ApprovalConstants
    {
        // ─── ApprovalRequest StatusId ─────────────────────────────────────
        public static class RequestStatus
        {
            public const int Draft      = 0;
            public const int Pending    = 1;
            public const int InProgress = 2;
            public const int Approved   = 3;
            public const int Rejected   = 4;
            public const int Cancelled  = 5;

            public static string GetName(int statusId) => statusId switch
            {
                Draft      => "Draft",
                Pending    => "Pending",
                InProgress => "In Progress",
                Approved   => "Approved",
                Rejected   => "Rejected",
                Cancelled  => "Cancelled",
                _          => "Unknown"
            };
        }

        // ─── ApprovalStep StatusId ────────────────────────────────────────
        public static class StepStatus
        {
            public const int Waiting   = 0;
            public const int Pending   = 1;
            public const int Approved  = 2;
            public const int Rejected  = 3;
            public const int Skipped   = 4;
            public const int Escalated = 5;
            public const int Delegated = 6;

            public static string GetName(int statusId) => statusId switch
            {
                Waiting   => "Waiting",
                Pending   => "Pending",
                Approved  => "Approved",
                Rejected  => "Rejected",
                Skipped   => "Skipped",
                Escalated => "Escalated",
                Delegated => "Delegated",
                _         => "Unknown"
            };
        }

        // ─── ApproverType ─────────────────────────────────────────────────
        public static class ApproverType
        {
            public const string SpecificUser  = "SPECIFIC_USER";
            public const string UserGroup     = "USER_GROUP";
            public const string DirectManager = "DIRECT_MANAGER";   // via OrganizationUnit.LeaderUserId
        }

        // ─── History ActionType ───────────────────────────────────────────
        public static class ActionType
        {
            public const string Submitted = "SUBMITTED";
            public const string Approved  = "APPROVED";
            public const string Rejected  = "REJECTED";
            public const string Delegated = "DELEGATED";
            public const string Escalated = "ESCALATED";
            public const string Cancelled = "CANCELLED";
            public const string Skipped   = "SKIPPED";
            public const string Reminded  = "REMINDED";
        }
    }
}
