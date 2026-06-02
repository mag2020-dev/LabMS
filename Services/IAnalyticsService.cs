namespace LabMS.Services;

public interface IAnalyticsService
{
    Task<DashboardResponse> GetDashboardMetricsAsync();
    Task<FinancialReportResponse> GetFinancialReportAsync(DateTime startDate, DateTime endDate);
    Task<OperationalReportResponse> GetOperationalReportAsync(DateTime startDate, DateTime endDate);
    Task<ClinicalReportResponse> GetClinicalReportAsync(DateTime startDate, DateTime endDate);
    Task<InventoryReportResponse> GetInventoryReportAsync(DateTime startDate, DateTime endDate);
}
