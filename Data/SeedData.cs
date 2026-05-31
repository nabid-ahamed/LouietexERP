using LouietexERP.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using LouietexERP.Controllers;

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
            await SeedActivityLogsAsync(context);
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

            var employees = new List<Employee>();
            var firstNames = new[] { "Md. Rafiqul", "Fatema", "Md. Karim", "Sumaiya", "Md. Jahangir", "Rehana", "Md. Arif", "Nargis", "Md. Shafiqul", "Sabina", "Md. Robiul", "Taslima", "Md. Nurul", "Halima", "Md. Zakir", "Roksana", "Md. Anwar", "Jesmin", "Md. Sohel", "Champa", "Md. Delwar", "Nasrin", "Abul", "Farhana", "Mizanur", "Lucky", "Shahidul", "Munmun", "Habibur", "Rozina", "Kamrul", "Shamima" };
            var lastNames = new[] { "Islam", "Akter", "Hossain", "Begum", "Alam", "Parvin", "Billah", "Sultana", "Haque", "Yasmin", "Awal", "Khanam", "Khatun", "Rana", "Khan", "Ahmed", "Rahman" };
            var departments = new[] { "Cutting", "Sewing", "Finishing", "QC", "HR", "Production", "Merchandising", "Packing", "Embroidery" };
            var roles = new[] { "Operator", "Supervisor", "Manager", "QC Officer", "Helper", "Line Chief", "HR Officer", "Merchandiser" };

            var random = new Random();
            for (int i = 0; i < 35; i++)
            {
                var fName = firstNames[i % firstNames.Length];
                var lName = lastNames[random.Next(lastNames.Length)];
                var fullName = $"{fName} {lName}";
                var email = $"{fName.ToLower().Replace(".", "").Replace(" ", "")}.{lName.ToLower()}@louietex.com";

                employees.Add(new Employee
                {
                    FullName = fullName,
                    Email = email,
                    Department = departments[random.Next(departments.Length)],
                    Role = roles[random.Next(roles.Length)],
                    JoiningDate = DateTime.Now.AddDays(-random.Next(30, 1500))
                });
            }

            await context.Employees.AddRangeAsync(employees);
            await context.SaveChangesAsync();
        }

        private static async Task SeedOrdersAsync(ApplicationDbContext context)
        {
            if (await context.Orders.AnyAsync()) return;

            var now = DateTime.UtcNow;
            var bulkBuyers = new[] { "Uniqlo", "GAP", "Next", "Primark", "M&S", "Puma" };
            var categories = new[] { "TS", "PL", "JK", "DN", "SW", "HD" };
            var bulkStatuses = new[] { "Pending", "In Production", "Shipped", "Completed" };
            var randomBulk = new Random();

            for (int i = 0; i < 25; i++)
            {
                var buyer = bulkBuyers[randomBulk.Next(bulkBuyers.Length)];
                var cat = categories[randomBulk.Next(categories.Length)];
                var orderDate = now.AddDays(-randomBulk.Next(10, 400));

                context.Orders.Add(new Order
                {
                    BuyerName = buyer,
                    StyleCode = $"{buyer[..2].ToUpper()}-{cat}-25{i:D2}",
                    TotalQuantity = randomBulk.Next(1, 5) * 1000,
                    DeliveryDate = orderDate.AddDays(randomBulk.Next(30, 90)),
                    Status = bulkStatuses[randomBulk.Next(bulkStatuses.Length)],
                    CreatedAt = orderDate
                });
            }
            await context.SaveChangesAsync();
        }

        private static async Task SeedProductionsAsync(ApplicationDbContext context)
        {
            if (await context.Productions.AnyAsync()) return;

            var orders = await context.Orders.ToListAsync();
            if (orders.Count == 0) return;

            var supervisorNames = await context.Employees
                .Where(e => e.Role == "Supervisor" || e.Role == "Manager" || e.Role == "Line Chief")
                .Select(e => e.FullName)
                .ToListAsync();
            if (supervisorNames.Count == 0) supervisorNames.Add("Md. Shafiqul Haque");

            var now = DateTime.UtcNow;
            var random = new Random();
            var prods = new List<Production>();

            foreach (var order in orders)
            {
                var line = $"Line-{random.Next(1, 10):D2}";
                var supervisor = supervisorNames[random.Next(supervisorNames.Count)];
                var target = order.TotalQuantity / 2;
                var status = order.Status == "Completed" ? "Completed" : (order.Status == "Shipped" ? "Completed" : "Running");
                var actual = status == "Completed" ? target : (int)(target * random.NextDouble());

                var start = order.CreatedAt.AddDays(random.Next(1, 5));
                var end = status == "Completed" ? start.AddDays(random.Next(5, 15)) : (DateTime?)null;

                prods.Add(new Production
                {
                    OrderId = order.Id,
                    LineNumber = line,
                    Supervisor = supervisor,
                    TargetQuantity = target,
                    ActualOutput = actual,
                    DefectCount = (int)(actual * 0.015),
                    StartDate = start,
                    EndDate = end,
                    Status = status,
                    CreatedAt = start,
                    UpdatedAt = now
                });
            }

            await context.Productions.AddRangeAsync(prods);
            await context.SaveChangesAsync();
        }

        private static async Task SeedQCAsync(ApplicationDbContext context)
        {
            if (await context.QCInspections.AnyAsync()) return;

            var productions = await context.Productions.ToListAsync();
            if (productions.Count == 0) return;

            var inspectorNames = await context.Employees
                .Where(e => e.Role == "QC Officer" || e.Role == "QC Inspector")
                .Select(e => e.FullName)
                .ToListAsync();
            if (inspectorNames.Count == 0) inspectorNames.AddRange(["Abdul Karim", "Mohammad Rafiq", "Suraiya Begum"]);

            var now = DateTime.UtcNow;
            var random = new Random();
            var qcRecords = new List<QCInspection>();

            for (int i = 0; i < 40; i++)
            {
                var prod = productions[random.Next(productions.Count)];
                var inspector = inspectorNames[random.Next(inspectorNames.Count)];

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

                var date = prod.CreatedAt.AddDays(random.Next(1, 10));
                qcRecords.Add(new QCInspection
                {
                    ProductionId = prod.Id,
                    InspectorName = inspector,
                    DefectCount = defects,
                    QCStatus = status,
                    Remarks = remarks,
                    InspectionDate = date,
                    CreatedAt = date,
                    UpdatedAt = date
                });
            }

            await context.QCInspections.AddRangeAsync(qcRecords);
            await context.SaveChangesAsync();
        }

        private static async Task SeedActivityLogsAsync(ApplicationDbContext context)
        {
            if (await context.ActivityLogs.AnyAsync()) return;

            var logs = new List<ActivityLog>
            {
                new() { Title = "ERP System Seeded", Subtitle = "All core business modules initialized successfully.", Timestamp = DateTime.Now.AddHours(-6), Icon = "bi-cpu", IconBg = "bg-primary-subtle", IconText = "text-primary", Module = "System" },
                new() { Title = "New Order: GA-DN-2515", Subtitle = "Buyer: GAP | Quantity: 8,500", Timestamp = DateTime.Now.AddHours(-4), Icon = "bi-bag-plus", IconBg = "bg-primary-subtle", IconText = "text-primary", Module = "Orders" },
                new() { Title = "Production Running: GA-DN-2515", Subtitle = "Line: Line-06 | Supervisor: Abul Rahman", Timestamp = DateTime.Now.AddHours(-3), Icon = "bi-play-circle", IconBg = "bg-info-subtle", IconText = "text-info", Module = "Production" },
                new() { Title = "QC Passed: GA-DN-2515", Subtitle = "Inspector: QC Inspector Pro | Defects: 5", Timestamp = DateTime.Now.AddHours(-2), Icon = "bi-shield-check", IconBg = "bg-success-subtle", IconText = "text-success", Module = "QC" },
                new() { Title = "Inventory Created: Cotton Fabric (White)", Subtitle = "Initial quantity: 5,000 units added.", Timestamp = DateTime.Now.AddDays(-1), Icon = "bi-box-seam", IconBg = "bg-warning-subtle", IconText = "text-warning", Module = "Inventory" }
            };

            await context.ActivityLogs.AddRangeAsync(logs);
            await context.SaveChangesAsync();
        }
    }
}
