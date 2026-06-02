namespace LabMS.Services;

public class SampleService(LabMSDbContext context, IMapper mapper) : ISampleService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<SampleResponse?> CreateAsync(SampleCreateRequest request)
    {
        // Check if sample code already exists
        var exists = await _context.Samples.AnyAsync(s => s.SampleCode == request.SampleCode);
        if (exists) return null;

        var sample = new Sample
        {
            SampleCode = request.SampleCode,
            TestOrderId = request.TestOrderId,
            Type = request.Type,
            Status = SampleStatus.Collected,
            CollectedAt = DateTime.UtcNow,
            CollectedByUserId = request.CollectedByUserId,
            StorageLocation = request.StorageLocation,
            Container = request.Container,
            Volume = request.Volume,
            VolumeUnit = request.VolumeUnit,
            Notes = request.Notes
        };

        _context.Samples.Add(sample);

        // Add tracking entry
        sample.TrackingHistory.Add(new SampleTracking
        {
            SampleId = sample.Id,
            Status = SampleStatus.Collected,
            UserId = request.CollectedByUserId,
            Location = request.StorageLocation,
            Notes = "Sample collected"
        });

        await _context.SaveChangesAsync();
        return await GetByIdAsync(sample.Id);
    }

    public async Task<SampleResponse?> GetByIdAsync(Guid id)
    {
        var sample = await _context.Samples
            .Include(s => s.TestOrder)
                .ThenInclude(to => to.LabTest)
            .Include(s => s.TestOrder)
                .ThenInclude(to => to.Visit)
                .ThenInclude(v => v.Patient)
            .Include(s => s.CollectedBy)
            .Include(s => s.ReceivedBy)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sample is null) return null;

        return MapToResponse(sample);
    }

    public async Task<SampleResponse?> GetBySampleCodeAsync(string sampleCode)
    {
        var sample = await _context.Samples
            .Include(s => s.TestOrder)
                .ThenInclude(to => to.LabTest)
            .Include(s => s.TestOrder)
                .ThenInclude(to => to.Visit)
                .ThenInclude(v => v.Patient)
            .Include(s => s.CollectedBy)
            .Include(s => s.ReceivedBy)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SampleCode == sampleCode);

        if (sample is null) return null;

        return MapToResponse(sample);
    }

    public async Task<IEnumerable<SampleResponse>> GetAllAsync()
    {
        var samples = await _context.Samples
            .Include(s => s.TestOrder)
                .ThenInclude(to => to.LabTest)
            .Include(s => s.TestOrder)
                .ThenInclude(to => to.Visit)
                .ThenInclude(v => v.Patient)
            .Include(s => s.CollectedBy)
            .Include(s => s.ReceivedBy)
            .OrderByDescending(s => s.CollectedAt)
            .AsNoTracking()
            .ToListAsync();

        return samples.Select(MapToResponse);
    }

    public async Task<IEnumerable<SampleResponse>> GetByTestOrderIdAsync(Guid testOrderId)
    {
        var samples = await _context.Samples
            .Include(s => s.TestOrder)
                .ThenInclude(to => to.LabTest)
            .Include(s => s.TestOrder)
                .ThenInclude(to => to.Visit)
                .ThenInclude(v => v.Patient)
            .Include(s => s.CollectedBy)
            .Include(s => s.ReceivedBy)
            .Where(s => s.TestOrderId == testOrderId)
            .OrderByDescending(s => s.CollectedAt)
            .AsNoTracking()
            .ToListAsync();

        return samples.Select(MapToResponse);
    }

    public async Task<IEnumerable<SampleResponse>> GetByStatusAsync(SampleStatus status)
    {
        var samples = await _context.Samples
            .Include(s => s.TestOrder)
                .ThenInclude(to => to.LabTest)
            .Include(s => s.TestOrder)
                .ThenInclude(to => to.Visit)
                .ThenInclude(v => v.Patient)
            .Include(s => s.CollectedBy)
            .Include(s => s.ReceivedBy)
            .Where(s => s.Status == status)
            .OrderByDescending(s => s.CollectedAt)
            .AsNoTracking()
            .ToListAsync();

        return samples.Select(MapToResponse);
    }

    public async Task<IEnumerable<SampleResponse>> GetByPatientIdAsync(Guid patientId)
    {
        var samples = await _context.Samples
            .Include(s => s.TestOrder)
                .ThenInclude(to => to.LabTest)
            .Include(s => s.TestOrder)
                .ThenInclude(to => to.Visit)
                .ThenInclude(v => v.Patient)
            .Include(s => s.CollectedBy)
            .Include(s => s.ReceivedBy)
            .Where(s => s.TestOrder.Visit.PatientId == patientId)
            .OrderByDescending(s => s.CollectedAt)
            .AsNoTracking()
            .ToListAsync();

        return samples.Select(MapToResponse);
    }

    public async Task<bool> UpdateAsync(Guid id, SampleUpdateRequest request)
    {
        var sample = await _context.Samples
            .Include(s => s.TrackingHistory)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sample is null) return false;

        var statusChanged = false;
        var oldStatus = sample.Status;

        if (request.Status.HasValue && request.Status.Value != sample.Status)
        {
            sample.Status = request.Status.Value;
            statusChanged = true;
        }

        if (request.StorageLocation != null) sample.StorageLocation = request.StorageLocation;
        if (request.Container != null) sample.Container = request.Container;
        if (request.Volume.HasValue) sample.Volume = request.Volume;
        if (request.VolumeUnit != null) sample.VolumeUnit = request.VolumeUnit;
        if (request.Notes != null) sample.Notes = request.Notes;

        // Add tracking entry if status changed
        if (statusChanged)
        {
            sample.TrackingHistory.Add(new SampleTracking
            {
                SampleId = sample.Id,
                Status = sample.Status,
                Location = sample.StorageLocation,
                Notes = $"Status changed from {oldStatus} to {sample.Status}"
            });
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReceiveAsync(Guid id, SampleReceiveRequest request)
    {
        var sample = await _context.Samples
            .Include(s => s.TrackingHistory)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sample is null) return false;

        sample.Status = SampleStatus.Received;
        sample.ReceivedAt = DateTime.UtcNow;
        sample.ReceivedByUserId = request.ReceivedByUserId;
        if (request.StorageLocation != null)
            sample.StorageLocation = request.StorageLocation;

        sample.TrackingHistory.Add(new SampleTracking
        {
            SampleId = sample.Id,
            Status = SampleStatus.Received,
            UserId = request.ReceivedByUserId,
            Location = sample.StorageLocation,
            Notes = "Sample received at lab"
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectAsync(Guid id, SampleRejectRequest request)
    {
        var sample = await _context.Samples
            .Include(s => s.TrackingHistory)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sample is null) return false;

        sample.Status = SampleStatus.Rejected;
        sample.RejectionReason = request.RejectionReason;
        sample.RejectedAt = DateTime.UtcNow;

        sample.TrackingHistory.Add(new SampleTracking
        {
            SampleId = sample.Id,
            Status = SampleStatus.Rejected,
            UserId = request.RejectedByUserId,
            Notes = $"Sample rejected: {request.RejectionReason}"
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAsProcessedAsync(Guid id)
    {
        var sample = await _context.Samples
            .Include(s => s.TrackingHistory)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sample is null) return false;

        sample.Status = SampleStatus.Tested;
        sample.ProcessedAt = DateTime.UtcNow;

        sample.TrackingHistory.Add(new SampleTracking
        {
            SampleId = sample.Id,
            Status = SampleStatus.Tested,
            Notes = "Sample testing completed"
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAsDisposedAsync(Guid id)
    {
        var sample = await _context.Samples
            .Include(s => s.TrackingHistory)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (sample is null) return false;

        sample.Status = SampleStatus.Disposed;
        sample.DisposedAt = DateTime.UtcNow;

        sample.TrackingHistory.Add(new SampleTracking
        {
            SampleId = sample.Id,
            Status = SampleStatus.Disposed,
            Notes = "Sample disposed"
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<SampleTrackingResponse>> GetTrackingHistoryAsync(Guid sampleId)
    {
        var tracking = await _context.SampleTrackings
            .Include(st => st.User)
            .Where(st => st.SampleId == sampleId)
            .OrderBy(st => st.Timestamp)
            .AsNoTracking()
            .ToListAsync();

        return tracking.Select(t => new SampleTrackingResponse(
            t.Id,
            t.SampleId,
            t.Status,
            t.Timestamp,
            t.User?.Username,
            t.Location,
            t.Notes
        ));
    }

    public async Task<SampleStatisticsResponse> GetStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _context.Samples.AsQueryable();

        if (startDate.HasValue)
            query = query.Where(s => s.CollectedAt >= startDate.Value);

        if (endDate.HasValue)
            query = query.Where(s => s.CollectedAt <= endDate.Value);

        var samples = await query.ToListAsync();

        var samplesByType = samples
            .GroupBy(s => s.Type)
            .ToDictionary(g => g.Key, g => g.Count());

        var samplesByStatus = samples
            .GroupBy(s => s.Status)
            .ToDictionary(g => g.Key, g => g.Count());

        return new SampleStatisticsResponse(
            samples.Count,
            samples.Count(s => s.Status == SampleStatus.Collected),
            samples.Count(s => s.Status == SampleStatus.InTransit),
            samples.Count(s => s.Status == SampleStatus.Received),
            samples.Count(s => s.Status == SampleStatus.InTesting),
            samples.Count(s => s.Status == SampleStatus.Tested),
            samples.Count(s => s.Status == SampleStatus.Rejected),
            samplesByType,
            samplesByStatus
        );
    }

    private static SampleResponse MapToResponse(Sample sample)
    {
        return new SampleResponse(
            sample.Id,
            sample.SampleCode,
            sample.TestOrderId,
            sample.TestOrder.LabTest.Name,
            $"{sample.TestOrder.Visit.Patient.FirstName} {sample.TestOrder.Visit.Patient.LastName}",
            sample.Type,
            sample.Status,
            sample.CollectedAt,
            sample.CollectedBy?.Username,
            sample.ReceivedAt,
            sample.ReceivedBy?.Username,
            sample.StorageLocation,
            sample.Container,
            sample.Volume,
            sample.VolumeUnit,
            sample.ProcessedAt,
            sample.DisposedAt,
            sample.RejectionReason,
            sample.RejectedAt,
            sample.Notes
        );
    }
}
