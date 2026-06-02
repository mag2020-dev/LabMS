namespace LabMS.Contracts.Analytics;

// Main Dashboard Response
public record DashboardResponse(
    FinancialDashboardMetrics Financials,
    OperationalDashboardMetrics Operations,
    ClinicalDashboardMetrics Clinical,
    InventoryDashboardMetrics Inventory
);

public record FinancialDashboardMetrics(
    decimal TotalRevenue,
    decimal RevenueToday,
    decimal OutstandingReceivables,
    int InvoicesGenerated,
    int PaidInvoices
);

public record OperationalDashboardMetrics(
    int TotalTestsToday,
    double AverageTurnaroundTimeHours,
    int SamplesCollectedToday,
    double SampleRejectionRate
);

public record ClinicalDashboardMetrics(
    int AbnormalResultsToday,
    string MostCommonTestToday
);

public record InventoryDashboardMetrics(
    int LowStockItems,
    int ExpiredItems,
    decimal TotalInventoryValue
);

// Detailed Reports
public record FinancialReportResponse(
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalRevenue,
    decimal TotalDiscounts,
    decimal TotalTaxes,
    decimal TotalPayments,
    decimal NetIncome,
    Dictionary<string, decimal> RevenueByTest,
    Dictionary<string, decimal> RevenueByPackage
);

public record OperationalReportResponse(
    DateTime StartDate,
    DateTime EndDate,
    int TotalTests,
    double AverageTurnaroundTimeHours,
    int TotalSamples,
    double SampleRejectionRate,
    Dictionary<string, int> TestsByTechnician
);

public record ClinicalReportResponse(
    DateTime StartDate,
    DateTime EndDate,
    int TotalAbnormalResults,
    Dictionary<string, int> TestFrequency,
    Dictionary<string, int> AbnormalResultsByTest
);

public record InventoryReportResponse(
    DateTime StartDate,
    DateTime EndDate,
    decimal TotalInventoryValue,
    decimal ValueOfStockAdded,
    decimal ValueOfStockUsed,
    int ExpiredItemsCount,
    decimal ExpiredItemsValue
);
