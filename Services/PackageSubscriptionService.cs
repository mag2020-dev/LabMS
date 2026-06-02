namespace LabMS.Services;

public class PackageSubscriptionService(LabMSDbContext context, IMapper mapper) : IPackageSubscriptionService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<PackageSubscriptionResponse?> CreateAsync(PackageSubscriptionCreateRequest request)
    {
        var subscription = new PackageSubscription
        {
            PatientId = request.PatientId,
            TestPackageId = request.TestPackageId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Frequency = request.Frequency,
            Status = SubscriptionStatus.Active,
            TestsRemaining = request.TestsIncluded
        };

        // Calculate monthly price based on package and frequency
        var package = await _context.TestPackages.FindAsync(request.TestPackageId);
        if (package is null) return null;

        subscription.MonthlyPrice = CalculateMonthlyPrice(package.PackagePrice, request.Frequency);

        _context.PackageSubscriptions.Add(subscription);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(subscription.Id);
    }

    public async Task<PackageSubscriptionResponse?> GetByIdAsync(Guid id)
    {
        var subscription = await _context.PackageSubscriptions
            .Include(s => s.Patient)
            .Include(s => s.TestPackage)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (subscription is null) return null;

        return MapToResponse(subscription);
    }

    public async Task<IEnumerable<PackageSubscriptionResponse>> GetAllAsync()
    {
        var subscriptions = await _context.PackageSubscriptions
            .Include(s => s.Patient)
            .Include(s => s.TestPackage)
            .OrderByDescending(s => s.CreatedAt)
            .AsNoTracking()
            .ToListAsync();

        return subscriptions.Select(MapToResponse);
    }

    public async Task<IEnumerable<PackageSubscriptionResponse>> GetByPatientIdAsync(Guid patientId)
    {
        var subscriptions = await _context.PackageSubscriptions
            .Include(s => s.Patient)
            .Include(s => s.TestPackage)
            .Where(s => s.PatientId == patientId)
            .OrderByDescending(s => s.CreatedAt)
            .AsNoTracking()
            .ToListAsync();

        return subscriptions.Select(MapToResponse);
    }

    public async Task<IEnumerable<PackageSubscriptionResponse>> GetActiveSubscriptionsAsync()
    {
        var subscriptions = await _context.PackageSubscriptions
            .Include(s => s.Patient)
            .Include(s => s.TestPackage)
            .Where(s => s.Status == SubscriptionStatus.Active && s.EndDate >= DateTime.UtcNow)
            .OrderBy(s => s.EndDate)
            .AsNoTracking()
            .ToListAsync();

        return subscriptions.Select(MapToResponse);
    }

    public async Task<bool> CancelAsync(Guid id, string reason)
    {
        var subscription = await _context.PackageSubscriptions.FindAsync(id);
        if (subscription is null) return false;

        subscription.Status = SubscriptionStatus.Cancelled;
        subscription.CancelledAt = DateTime.UtcNow;
        subscription.CancellationReason = reason;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> PauseAsync(Guid id)
    {
        var subscription = await _context.PackageSubscriptions.FindAsync(id);
        if (subscription is null) return false;

        subscription.Status = SubscriptionStatus.Paused;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ResumeAsync(Guid id)
    {
        var subscription = await _context.PackageSubscriptions.FindAsync(id);
        if (subscription is null) return false;

        subscription.Status = SubscriptionStatus.Active;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UseTestAsync(Guid subscriptionId)
    {
        var subscription = await _context.PackageSubscriptions.FindAsync(subscriptionId);
        if (subscription is null || subscription.TestsRemaining <= 0) return false;

        subscription.TestsRemaining--;
        await _context.SaveChangesAsync();
        return true;
    }

    private static decimal CalculateMonthlyPrice(decimal packagePrice, SubscriptionFrequency frequency)
    {
        return frequency switch
        {
            SubscriptionFrequency.Weekly => packagePrice * 4,
            SubscriptionFrequency.BiWeekly => packagePrice * 2,
            SubscriptionFrequency.Monthly => packagePrice,
            SubscriptionFrequency.Quarterly => packagePrice / 3,
            SubscriptionFrequency.Annually => packagePrice / 12,
            _ => packagePrice
        };
    }

    private static PackageSubscriptionResponse MapToResponse(PackageSubscription subscription)
    {
        return new PackageSubscriptionResponse(
            subscription.Id,
            subscription.PatientId,
            $"{subscription.Patient.FirstName} {subscription.Patient.LastName}",
            subscription.TestPackageId,
            subscription.TestPackage.Name,
            subscription.StartDate,
            subscription.EndDate,
            subscription.Frequency,
            subscription.Status,
            subscription.MonthlyPrice,
            subscription.TestsRemaining,
            subscription.CreatedAt
        );
    }
}
