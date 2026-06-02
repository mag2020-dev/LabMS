namespace LabMS.Services;

public interface IPackageSubscriptionService
{
    Task<PackageSubscriptionResponse?> CreateAsync(PackageSubscriptionCreateRequest request);
    Task<PackageSubscriptionResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<PackageSubscriptionResponse>> GetAllAsync();
    Task<IEnumerable<PackageSubscriptionResponse>> GetByPatientIdAsync(Guid patientId);
    Task<IEnumerable<PackageSubscriptionResponse>> GetActiveSubscriptionsAsync();
    Task<bool> CancelAsync(Guid id, string reason);
    Task<bool> PauseAsync(Guid id);
    Task<bool> ResumeAsync(Guid id);
    Task<bool> UseTestAsync(Guid subscriptionId);
}
