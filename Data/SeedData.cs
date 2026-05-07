using LouietexERP.Models;
using Microsoft.EntityFrameworkCore;

namespace LouietexERP.Data
{
    public static class SeedData
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            await SeedInventoryAsync(context);
            await SeedEmployeesAsync(context);
            await SeedOrdersAsync(context);
            await SeedProductionsAsync(context);
            await SeedQCAsync(context);
        }

        private static async Task SeedInventoryAsync(ApplicationDbContext context)
        {
            if (await context.Inventories.AnyAsync()) return;

            var items = new List<Inventory>
            {
                new() { Name = "Cotton Fabric (White)", Category = "Fabric", Quantity = 5000, MinStockLevel = 1000, Supplier = "Chittagong Textile Mills" },
                new() { Name = "Cotton Fabric (Navy)", Category = "Fabric", Quantity = 3200, MinStockLevel = 800, Supplier = "Chittagong Textile Mills" },
                new() { Name = "Polyester Blend (Grey)", Category = "Fabric", Quantity = 2800, MinStockLevel = 700, Supplier = "Dhaka Fabrics Ltd" },
                new() { Name = "Denim Fabric (Blue)", Category = "Fabric", Quantity = 4100, MinStockLevel = 1000, Supplier = "Gazipur Denim Co" },
                new() { Name = "Fleece Fabric (Black)", Category = "Fabric", Quantity = 1500, MinStockLevel = 600, Supplier = "Dhaka Fabrics Ltd" },
                new() { Name = "Linen (Natural)", Category = "Fabric", Quantity = 900, MinStockLevel = 500, Supplier = "Narayanganj Textile" },
                new() { Name = "Jersey Knit (Red)", Category = "Fabric", Quantity = 600, MinStockLevel = 800, Supplier = "Chittagong Textile Mills" },
                new() { Name = "Sewing Thread (White, 5000m)", Category = "Accessories", Quantity = 320, MinStockLevel = 100, Supplier = "Coats Bangladesh" },
                new() { Name = "Sewing Thread (Black, 5000m)", Category = "Accessories", Quantity = 280, MinStockLevel = 100, Supplier = "Coats Bangladesh" },
                new() { Name = "Zipper (Metal, 20cm)", Category = "Accessories", Quantity = 4500, MinStockLevel = 1000, Supplier = "YKK Bangladesh" },
                new() { Name = "Zipper (Plastic, 15cm)", Category = "Accessories", Quantity = 6200, MinStockLevel = 1500, Supplier = "YKK Bangladesh" },
                new() { Name = "Button (4-hole, 15mm)", Category = "Accessories", Quantity = 45000, MinStockLevel = 10000, Supplier = "Galaxy Button Co" },
                new() { Name = "Elastic Band (25mm)", Category = "Accessories", Quantity = 8500, MinStockLevel = 2000, Supplier = "Narayanganj Textile" },
                new() { Name = "Interlining (Light)", Category = "Accessories", Quantity = 1200, MinStockLevel = 400, Supplier = "Dhaka Fabrics Ltd" },
                new() { Name = "Velcro Tape (2.5cm)", Category = "Accessories", Quantity = 3000, MinStockLevel = 500, Supplier = "Galaxy Button Co" },
                new() { Name = "Main Label (Woven)", Category = "Packaging", Quantity = 25000, MinStockLevel = 5000, Supplier = "BD Label House" },
                new() { Name = "Care Label (Printed)", Category = "Packaging", Quantity = 30000, MinStockLevel = 5000, Supplier = "BD Label House" },
                new() { Name = "Hang Tag", Category = "Packaging", Quantity = 12000, MinStockLevel = 3000, Supplier = "Print World BD" },
                new() { Name = "Polybag (40x60cm)", Category = "Packaging", Quantity = 850, MinStockLevel = 2000, Supplier = "Rupali Poly Industries" },
                new() { Name = "Carton Box (Export Grade)", Category = "Packaging", Quantity = 380, MinStockLevel = 500, Supplier = "Rupali Poly Industries" },
                new() { Name = "Shoulder Pad (Small)", Category = "Accessories", Quantity = 2200, MinStockLevel = 500, Supplier = "Dhaka Fabrics Ltd" },
                new() { Name = "Waistband Canvas", Category = "Fabric", Quantity = 1800, MinStockLevel = 400, Supplier = "Gazipur Denim Co" },
                new() { Name = "Embroidery Thread Set", Category = "Accessories", Quantity = 95, MinStockLevel = 50, Supplier = "Coats Bangladesh" },
                new() { Name = "Pocketing Fabric", Category = "Fabric", Quantity = 2600, MinStockLevel = 600, Supplier = "Narayanganj Textile" },
                new() { Name = "Collar Fusing", Category = "Accessories", Quantity = 420, MinStockLevel = 300, Supplier = "Dhaka Fabrics Ltd" },
            };
            await context.Inventories.AddRangeAsync(items);
            await context.SaveChangesAsync();
        }

        private static async Task SeedEmployeesAsync(ApplicationDbContext context)
        {
            if (await context.Employees.AnyAsync()) return;

            var employees = new List<Employee>
            {
                new() { FullName = "Md. Rafiqul Islam", Email = "rafiqul@louietex.com", Department = "Cutting", Role = "Supervisor", JoiningDate = new DateTime(2019, 3, 15) },
                new() { FullName = "Fatema Akter", Email = "fatema@louietex.com", Department = "Sewing", Role = "Operator", JoiningDate = new DateTime(2020, 6, 1) },
                new() { FullName = "Md. Karim Hossain", Email = "karim@louietex.com", Department = "Finishing", Role = "Supervisor", JoiningDate = new DateTime(2018, 11, 20) },
                new() { FullName = "Sumaiya Begum", Email = "sumaiya@louietex.com", Department = "QC", Role = "QC Officer", JoiningDate = new DateTime(2021, 2, 10) },
                new() { FullName = "Md. Jahangir Alam", Email = "jahangir@louietex.com", Department = "Sewing", Role = "Operator", JoiningDate = new DateTime(2022, 4, 5) },
                new() { FullName = "Rehana Parvin", Email = "rehana@louietex.com", Department = "Embroidery", Role = "Operator", JoiningDate = new DateTime(2020, 9, 14) },
                new() { FullName = "Md. Arif Billah", Email = "arif@louietex.com", Department = "Cutting", Role = "Operator", JoiningDate = new DateTime(2023, 1, 22) },
                new() { FullName = "Nargis Sultana", Email = "nargis@louietex.com", Department = "Packing", Role = "Operator", JoiningDate = new DateTime(2021, 7, 30) },
                new() { FullName = "Md. Shafiqul Haque", Email = "shafiq@louietex.com", Department = "Production", Role = "Manager", JoiningDate = new DateTime(2017, 5, 8) },
                new() { FullName = "Sabina Yasmin", Email = "sabina@louietex.com", Department = "HR", Role = "HR Officer", JoiningDate = new DateTime(2019, 8, 19) },
                new() { FullName = "Md. Robiul Awal", Email = "robiul@louietex.com", Department = "Sewing", Role = "Operator", JoiningDate = new DateTime(2022, 11, 3) },
                new() { FullName = "Taslima Khanam", Email = "taslima@louietex.com", Department = "Finishing", Role = "Operator", JoiningDate = new DateTime(2020, 3, 27) },
                new() { FullName = "Md. Nurul Islam", Email = "nurul@louietex.com", Department = "Merchandising", Role = "Merchandiser", JoiningDate = new DateTime(2018, 7, 12) },
                new() { FullName = "Halima Khatun", Email = "halima@louietex.com", Department = "QC", Role = "QC Officer", JoiningDate = new DateTime(2021, 9, 6) },
                new() { FullName = "Md. Zakir Hossain", Email = "zakir@louietex.com", Department = "Cutting", Role = "Helper", JoiningDate = new DateTime(2023, 3, 18) },
                new() { FullName = "Roksana Akter", Email = "roksana@louietex.com", Department = "Sewing", Role = "Operator", JoiningDate = new DateTime(2022, 6, 25) },
                new() { FullName = "Md. Anwar Hossain", Email = "anwar@louietex.com", Department = "Finishing", Role = "Helper", JoiningDate = new DateTime(2023, 5, 10) },
                new() { FullName = "Jesmin Akter", Email = "jesmin@louietex.com", Department = "Packing", Role = "Supervisor", JoiningDate = new DateTime(2019, 12, 2) },
                new() { FullName = "Md. Sohel Rana", Email = "sohel@louietex.com", Department = "Production", Role = "Line Chief", JoiningDate = new DateTime(2020, 1, 15) },
                new() { FullName = "Champa Begum", Email = "champa@louietex.com", Department = "Embroidery", Role = "Supervisor", JoiningDate = new DateTime(2018, 4, 22) },
                new() { FullName = "Md. Delwar Hossain", Email = "delwar@louietex.com", Department = "Sewing", Role = "Operator", JoiningDate = new DateTime(2024, 1, 8) },
                new() { FullName = "Nasrin Begum", Email = "nasrin@louietex.com", Department = "QC", Role = "QC Inspector", JoiningDate = new DateTime(2022, 8, 17) },
            };
            await context.Employees.AddRangeAsync(employees);
            await context.SaveChangesAsync();
        }

        private static async Task SeedOrdersAsync(ApplicationDbContext context)
        {
            if (await context.Orders.AnyAsync()) return;

            var now = DateTime.UtcNow;
            var orders = new List<Order>
            {
                new() { BuyerName = "H&M", StyleCode = "HM-TS-2401", TotalQuantity = 5000, DeliveryDate = now.AddDays(30), Status = "Pending", CreatedAt = now.AddDays(-5) },
                new() { BuyerName = "Zara", StyleCode = "ZR-PL-2402", TotalQuantity = 3200, DeliveryDate = now.AddDays(45), Status = "In Production", CreatedAt = now.AddDays(-20) },
                new() { BuyerName = "Uniqlo", StyleCode = "UQ-JK-2403", TotalQuantity = 2000, DeliveryDate = now.AddDays(60), Status = "In Production", CreatedAt = now.AddDays(-30) },
                new() { BuyerName = "GAP", StyleCode = "GAP-DN-2404", TotalQuantity = 4500, DeliveryDate = now.AddDays(-10), Status = "Completed", CreatedAt = now.AddDays(-60) },
                new() { BuyerName = "Next", StyleCode = "NXT-SW-2405", TotalQuantity = 1800, DeliveryDate = now.AddDays(-5), Status = "Shipped", CreatedAt = now.AddDays(-50) },
                new() { BuyerName = "H&M", StyleCode = "HM-HD-2406", TotalQuantity = 6000, DeliveryDate = now.AddDays(20), Status = "In Production", CreatedAt = now.AddDays(-15) },
                new() { BuyerName = "Primark", StyleCode = "PRM-TRS-2407", TotalQuantity = 8000, DeliveryDate = now.AddDays(75), Status = "Pending", CreatedAt = now.AddDays(-3) },
                new() { BuyerName = "Zara", StyleCode = "ZR-BZ-2408", TotalQuantity = 2500, DeliveryDate = now.AddDays(-20), Status = "Shipped", CreatedAt = now.AddDays(-70) },
                new() { BuyerName = "M&S", StyleCode = "MS-SK-2409", TotalQuantity = 3000, DeliveryDate = now.AddDays(15), Status = "In Production", CreatedAt = now.AddDays(-25) },
                new() { BuyerName = "GAP", StyleCode = "GAP-PL-2410", TotalQuantity = 3500, DeliveryDate = now.AddDays(-30), Status = "Completed", CreatedAt = now.AddDays(-80) },
                new() { BuyerName = "Uniqlo", StyleCode = "UQ-FLC-2411", TotalQuantity = 4000, DeliveryDate = now.AddDays(50), Status = "Pending", CreatedAt = now.AddDays(-8) },
                new() { BuyerName = "H&M", StyleCode = "HM-JN-2412", TotalQuantity = 5500, DeliveryDate = now.AddDays(-15), Status = "Completed", CreatedAt = now.AddDays(-90) },
                new() { BuyerName = "Next", StyleCode = "NXT-CT-2413", TotalQuantity = 1500, DeliveryDate = now.AddDays(35), Status = "Pending", CreatedAt = now.AddDays(-2) },
                new() { BuyerName = "Primark", StyleCode = "PRM-SH-2414", TotalQuantity = 7000, DeliveryDate = now.AddDays(-40), Status = "Shipped", CreatedAt = now.AddDays(-100) },
                new() { BuyerName = "M&S", StyleCode = "MS-DS-2415", TotalQuantity = 2200, DeliveryDate = now.AddDays(28), Status = "In Production", CreatedAt = now.AddDays(-18) },
                new() { BuyerName = "Zara", StyleCode = "ZR-TS-2416", TotalQuantity = 4800, DeliveryDate = now.AddDays(55), Status = "Pending", CreatedAt = now.AddDays(-4) },
                new() { BuyerName = "GAP", StyleCode = "GAP-JK-2417", TotalQuantity = 2800, DeliveryDate = now.AddDays(-50), Status = "Shipped", CreatedAt = now.AddDays(-110) },
                new() { BuyerName = "H&M", StyleCode = "HM-SW-2418", TotalQuantity = 3800, DeliveryDate = now.AddDays(40), Status = "In Production", CreatedAt = now.AddDays(-22) },
                new() { BuyerName = "Uniqlo", StyleCode = "UQ-TRS-2419", TotalQuantity = 6500, DeliveryDate = now.AddDays(-60), Status = "Completed", CreatedAt = now.AddDays(-120) },
                new() { BuyerName = "Next", StyleCode = "NXT-BZ-2420", TotalQuantity = 1200, DeliveryDate = now.AddDays(70), Status = "Pending", CreatedAt = now.AddDays(-1) },
                new() { BuyerName = "Primark", StyleCode = "PRM-JN-2421", TotalQuantity = 9000, DeliveryDate = now.AddDays(-35), Status = "Shipped", CreatedAt = now.AddDays(-95) },
                new() { BuyerName = "M&S", StyleCode = "MS-PL-2422", TotalQuantity = 2600, DeliveryDate = now.AddDays(22), Status = "In Production", CreatedAt = now.AddDays(-12) },
            };
            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();
        }

        private static async Task SeedProductionsAsync(ApplicationDbContext context)
        {
            if (await context.Productions.AnyAsync()) return;

            var orders = await context.Orders.ToListAsync();
            if (!orders.Any()) return;

            var now = DateTime.UtcNow;

            Production Prod(Order o, string line, string supervisor, int target, int actual, string status, int? daysAgo = null, int? daysAhead = null)
            {
                var start = now.AddDays(-(daysAgo ?? 20));
                var end = status == "Completed" ? start.AddDays(daysAhead ?? 15) : (DateTime?)null;
                return new Production
                {
                    OrderId = o.Id, LineNumber = line, Supervisor = supervisor,
                    TargetQuantity = target, ActualOutput = actual,
                    DefectCount = actual > 0 ? (int)(actual * 0.02) : 0,
                    StartDate = start, EndDate = end, Status = status,
                    CreatedAt = start, UpdatedAt = now
                };
            }

            Order OAt(int i) => orders[Math.Min(i, orders.Count - 1)];

            var prods = new List<Production>
            {
                Prod(OAt(1),  "Line-01", "Md. Shafiqul Haque", 3200, 2800, "Running",    20),
                Prod(OAt(2),  "Line-02", "Md. Sohel Rana",     2000, 1400, "Running",    25),
                Prod(OAt(5),  "Line-03", "Md. Rafiqul Islam",  6000, 5800, "Completed",  50, 20),
                Prod(OAt(3),  "Line-01", "Md. Shafiqul Haque", 4500, 4500, "Completed",  65, 25),
                Prod(OAt(4),  "Line-02", "Md. Sohel Rana",     1800, 1800, "Completed",  55, 18),
                Prod(OAt(7),  "Line-04", "Md. Rafiqul Islam",  2500, 2500, "Completed",  75, 22),
                Prod(OAt(8),  "Line-03", "Md. Shafiqul Haque", 3000, 2600, "Running",    18),
                Prod(OAt(9),  "Line-05", "Champa Begum",       3500, 3500, "Completed",  85, 30),
                Prod(OAt(11), "Line-04", "Md. Sohel Rana",     5500, 5500, "Completed",  95, 35),
                Prod(OAt(12), "Line-02", "Md. Rafiqul Islam",  1500, 1200, "Running",    6), // Recent
                Prod(OAt(14), "Line-01", "Md. Shafiqul Haque", 7000, 7000, "Completed", 105, 40),
                Prod(OAt(15), "Line-05", "Md. Sohel Rana",     2200, 1800, "Running",    5), // Recent
                Prod(OAt(17), "Line-03", "Champa Begum",       2800, 2800, "Completed", 115, 42),
                Prod(OAt(17), "Line-06", "Md. Rafiqul Islam",  3800, 3100, "Running",    4), // Recent
                Prod(OAt(18), "Line-04", "Md. Shafiqul Haque", 6500, 6500, "Completed", 125, 45),
                Prod(OAt(20), "Line-02", "Md. Sohel Rana",     9000, 9000, "Completed", 100, 38),
                Prod(OAt(21), "Line-05", "Champa Begum",       2600, 2400, "Running",    3), // Recent
                Prod(OAt(0),  "Line-06", "Md. Rafiqul Islam",  5000, 4200, "Running",    2), // Recent
                Prod(OAt(6),  "Line-01", "Md. Shafiqul Haque", 8000, 3500, "Running",    1), // Recent
                Prod(OAt(10), "Line-07", "Md. Sohel Rana",     4000, 0,    "Pending",     3),
                Prod(OAt(13), "Line-03", "Champa Begum",       1200, 0,    "Pending",     1),
                Prod(OAt(19), "Line-07", "Md. Rafiqul Islam",  1200, 0,    "Delayed",     8),
            };
            await context.Productions.AddRangeAsync(prods);
            await context.SaveChangesAsync();
        }

        private static async Task SeedQCAsync(ApplicationDbContext context)
        {
            // Clear existing for a clean refresh as requested
            var existing = await context.QCInspections.ToListAsync();
            if (existing.Any())
            {
                context.QCInspections.RemoveRange(existing);
                await context.SaveChangesAsync();
            }

            var productions = await context.Productions
                .Where(p => p.Status == "Completed" || p.Status == "Running")
                .ToListAsync();
            if (!productions.Any()) return;

            var now = DateTime.UtcNow;
            var bengaliNames = new[] { 
                "Abdul Karim", "Mohammad Rafiq", "Suraiya Begum", "Zahidul Islam", "Fatema Akter",
                "Abul Hasan", "Nazmul Haque", "Rehana Sultana", "Kamrul Hasan", "Shamima Nasrin",
                "Ariful Islam", "Taslima Begum", "Zahirul Alam", "Salma Akter", "Shahadat Hossain",
                "Morshed Alam", "Rasheda Begum", "Anwar Hossain", "Nasrin Akter", "Shafiqul Islam",
                "Parvin Akter", "Rozina Begum", "Jahangir Alam", "Rubi Akter", "Mizanur Rahman",
                "Shahidul Islam", "Lucky Akter", "Ahmed Ali", "Munmun Begum", "Habibur Rahman"
            };

            QCInspection QC(Production p, int defects, string status, string remarks, int daysAgo, string inspector)
            {
                var date = now.AddDays(-daysAgo);
                return new QCInspection
                {
                    ProductionId = p.Id,
                    InspectorName = inspector,
                    DefectCount = defects,
                    QCStatus = status,
                    Remarks = remarks,
                    InspectionDate = date,
                    CreatedAt = date,
                    UpdatedAt = date
                };
            }

            var random = new Random();
            var qcRecords = new List<QCInspection>();

            // Generate 30+ records
            for (int i = 0; i < 35; i++)
            {
                var prod = productions[random.Next(productions.Count)];
                var inspector = bengaliNames[i % bengaliNames.Length];
                
                // Randomize results
                int defectProb = random.Next(100);
                string status = "Passed";
                int defects = 0;
                string remarks = "No issues found during spot check.";

                if (defectProb < 15) {
                    status = "Failed";
                    defects = random.Next(15, 50);
                    remarks = "Critical stitching defects identified. Batch rejected.";
                } else if (defectProb < 30) {
                    status = "Recheck Required";
                    defects = random.Next(5, 15);
                    remarks = "Minor alignment issues. Re-inspection required after correction.";
                } else {
                    defects = random.Next(0, 5);
                    remarks = "QC passed. General quality standard maintained.";
                }

                qcRecords.Add(QC(prod, defects, status, remarks, random.Next(1, 90), inspector));
            }

            await context.QCInspections.AddRangeAsync(qcRecords);
            await context.SaveChangesAsync();
        }
    }
}
