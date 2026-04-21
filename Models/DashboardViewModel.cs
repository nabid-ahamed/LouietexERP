namespace LouietexERP.Models
{
    public class DashboardViewModel
    {
        public int TotalEmployees { get; set; }
        public int TotalOrders { get; set; }
        public int LowStockItems { get; set; }
        public int TodayProduction { get; set; }
        public double ProductionEfficiency { get; set; }
        public List<Order> RecentOrders { get; set; }
    }
}