namespace LouietexERP.Models
{
    public class DashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public int TotalOrders { get; set; }
        public int LowStockItems { get; set; }
        public int TodayProduction { get; set; }
        public double ProductionEfficiency { get; set; }
        public List<Order> RecentOrders { get; set; } = new();
        public int PendingUsers { get; set; }
        public int PendingRequests { get; set; }
        public List<int> MonthlyOrders { get; set; } = new();
        public List<int> MonthlyProduction { get; set; } = new();
        public List<string> MonthLabels { get; set; } = new();
    }
}