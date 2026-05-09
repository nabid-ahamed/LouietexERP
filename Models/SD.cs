namespace LouietexERP.Models
{
    public static class SD
    {
        public const string Role_SuperAdmin = "SuperAdmin";
        public const string Role_Admin = "Admin";
        public const string Role_HR = "HR";
        public const string Role_Merchandiser = "Merchandiser";
        public const string Role_ProductionManager = "ProductionManager";
        public const string Role_QC = "QC";
        public const string Role_OperationsManager = "OperationsManager";
        public const string Role_User = "User";

        public static string GetTimeAgo(DateTime dateTime)
        {
            var timeSpan = DateTime.Now - dateTime;

            if (timeSpan.TotalMinutes < 1) return "Just now";
            if (timeSpan.TotalMinutes < 60) return $"{(int)timeSpan.TotalMinutes} minutes ago";
            if (timeSpan.TotalHours < 24) return $"{(int)timeSpan.TotalHours} hours ago";
            if (timeSpan.TotalDays < 2) return "Yesterday";
            if (timeSpan.TotalDays < 7) return $"{(int)timeSpan.TotalDays} days ago";
            if (timeSpan.TotalDays < 30) return $"{(int)timeSpan.TotalDays / 7} weeks ago";

            return dateTime.ToString("MMM dd, yyyy");
        }
    }
}
