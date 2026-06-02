namespace LabMS.Services;

public class AnalyticsService(LabMSDbContext context) : IAnalyticsService
{
    private readonly LabMSDbContext _context = context;

    public async Task<DashboardResponse> GetDashboardMetricsAsync()
    {
        var today = DateTime.UtcNow.Date;

        // Financials
        var invoices = await _context.Invoices.Where(i => i.IssuedAt >= today).ToListAsync();
        var totalRevenue = invoices.Sum(i => i.TotalAmount - i.Discount);
        var outstandingReceivables = await _context.Invoices.Where(i => i.Status != "Paid").SumAsync(i => i.TotalAmount - i.Discount);

        // Operations
        var testOrdersToday = await _context.TestOrders.Where(to => to.OrderedAt >= today).ToListAsync();
        var completedTests = await _context.TestResults.Where(tr => tr.ResultDate >= today).ToListAsync();
        var avgTurnaround = completedTests.Any() 
            ? completedTests.Average(tr => (tr.ResultDate - tr.TestOrder.OrderedAt).TotalHours) 
            : 0;

        // Clinical
        var abnormalResults = completedTests.Count(tr => IsAbnormal(tr));
        var mostCommonTest = testOrdersToday.Any() 
            ? testOrdersToday.GroupBy(to => to.LabTest.Name).OrderByDescending(g => g.Count()).First().Key 
            : "N/A";

        // Inventory
        var lowStockItems = await _context.InventoryItems.CountAsync(i => i.IsActive && i.Quantity <= i.ReorderLevel);
        var expiredItems = await _context.InventoryItems.CountAsync(i => i.IsActive && i.ExpiryDate.HasValue && i.ExpiryDate <= DateTime.UtcNow);
        var inventoryValue = await _context.InventoryItems.SumAsync(i => i.Quantity * i.UnitPrice);

        return new DashboardResponse(
            new FinancialDashboardMetrics(totalRevenue, totalRevenue, outstandingReceivables, invoices.Count, invoices.Count(i => i.Status == "Paid")),
            new OperationalDashboardMetrics(testOrdersToday.Count, avgTurnaround, testOrdersToday.Count, 0), // Sample rejection rate needs more logic
            new ClinicalDashboardMetrics(abnormalResults, mostCommonTest),
            new InventoryDashboardMetrics(lowStockItems, expiredItems, inventoryValue)
        );
    }

    public async Task<FinancialReportResponse> GetFinancialReportAsync(DateTime startDate, DateTime endDate)
    {
        var invoices = await _context.Invoices
            .Include(i => i.Visit).ThenInclude(v => v.TestOrders).ThenInclude(to => to.LabTest)
            .Where(i => i.IssuedAt >= startDate && i.IssuedAt <= endDate)
            .ToListAsync();

        var payments = await _context.Payments
            .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
            .ToListAsync();

        var totalRevenue = invoices.Sum(i => i.TotalAmount);
        var totalDiscounts = invoices.Sum(i => i.Discount);
        var totalPayments = payments.Sum(p => p.Amount);

        var revenueByTest = invoices
            .SelectMany(i => i.Visit.TestOrders)
            .GroupBy(to => to.LabTest.Name)
            .ToDictionary(g => g.Key, g => g.Sum(to => to.LabTest.Price));

        return new FinancialReportResponse(
            startDate,
            endDate,
            totalRevenue,
            totalDiscounts,
            0, // Tax logic needs to be added
            totalPayments,
            totalPayments - totalDiscounts, // Simplified net income
            revenueByTest,
            new Dictionary<string, decimal>() // Package revenue needs more logic
        );
    }

    public async Task<OperationalReportResponse> GetOperationalReportAsync(DateTime startDate, DateTime endDate)
    {
        var testOrders = await _context.TestOrders
            .Include(to => to.TestResult)
            .Include(to => to.LabTest)
            .Where(to => to.OrderedAt >= startDate && to.OrderedAt <= endDate)
            .ToListAsync();

        var completedTests = testOrders.Where(to => to.TestResult != null);
        var avgTurnaround = completedTests.Any() 
            ? completedTests.Average(to => (to.TestResult!.ResultDate - to.OrderedAt).TotalHours) 
            : 0;

        var testsByTechnician = await _context.TestResults
            .Include(tr => tr.VerifiedBy)
            .Where(tr => tr.ResultDate >= startDate && tr.ResultDate <= endDate && tr.VerifiedBy != null)
            .GroupBy(tr => tr.VerifiedBy!.Username)
            .ToDictionaryAsync(g => g.Key, g => g.Count());

        return new OperationalReportResponse(
            startDate,
            endDate,
            testOrders.Count,
            avgTurnaround,
            testOrders.Count, // Simplified sample count
            0, // Sample rejection rate needs more logic
            testsByTechnician
        );
    }

    public async Task<ClinicalReportResponse> GetClinicalReportAsync(DateTime startDate, DateTime endDate)
    {
        var testResults = await _context.TestResults
            .Include(tr => tr.TestOrder).ThenInclude(to => to.LabTest)
            .Where(tr => tr.ResultDate >= startDate && tr.ResultDate <= endDate)
            .ToListAsync();

        var abnormalResults = testResults.Where(IsAbnormal).ToList();

        var testFrequency = testResults
            .GroupBy(tr => tr.TestOrder.LabTest.Name)
            .ToDictionary(g => g.Key, g => g.Count());

        var abnormalByTest = abnormalResults
            .GroupBy(tr => tr.TestOrder.LabTest.Name)
            .ToDictionary(g => g.Key, g => g.Count());

        return new ClinicalReportResponse(
            startDate,
            endDate,
            abnormalResults.Count,
            testFrequency,
            abnormalByTest
        );
    }

    public async Task<InventoryReportResponse> GetInventoryReportAsync(DateTime startDate, DateTime endDate)
    {
        var transactions = await _context.InventoryTransactions
            .Include(t => t.InventoryItem)
            .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
            .ToListAsync();

        var stockAdded = transactions.Where(t => t.Type == TransactionType.Purchase).Sum(t => t.Quantity * t.InventoryItem.UnitPrice);
        var stockUsed = transactions.Where(t => t.Type == TransactionType.Usage).Sum(t => t.Quantity * t.InventoryItem.UnitPrice);

        var expiredItems = await _context.InventoryItems
            .Where(i => i.ExpiryDate.HasValue && i.ExpiryDate >= startDate && i.ExpiryDate <= endDate)
            .ToListAsync();

        return new InventoryReportResponse(
            startDate,
            endDate,
            await _context.InventoryItems.SumAsync(i => i.Quantity * i.UnitPrice),
            stockAdded,
            stockUsed,
            expiredItems.Count,
            expiredItems.Sum(i => i.Quantity * i.UnitPrice)
        );
    }

    private bool IsAbnormal(TestResult result)
    {
        // This is a simplified check. A real implementation would parse the reference range.
        if (decimal.TryParse(result.ResultValue, out var value))
        {
            if (result.ReferenceRange != null)
            {
                var parts = result.ReferenceRange.Split('-');
                if (parts.Length == 2 && decimal.TryParse(parts[0], out var min) && decimal.TryParse(parts[1], out var max))
                {
                    return value < min || value > max;
                }
            }
        }
        return false;
    }
}
