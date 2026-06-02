

namespace LabMS.Services;

public class ReportService(LabMSDbContext context, IMapper mapper) : IReportService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<ReportResponse>> GetAllAsync()
    {
        var reports = await _context.Reports
            .Include(r => r.GeneratedBy)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<ReportResponse>>(reports);
    }

    public async Task<ReportResponse?> GetByIdAsync(Guid id)
    {
        var report = await _context.Reports
            .Include(r => r.GeneratedBy)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        return report is null ? null : _mapper.Map<ReportResponse>(report);
    }

    public async Task<ReportResponse?> AddAsync(ReportCreateRequest request)
    {
        var visit = await _context.Visits.FindAsync(request.VisitId);
        if (visit is null) return null;

        var report = _mapper.Map<Report>(request);
        _context.Reports.Add(report);
        await _context.SaveChangesAsync();

        return _mapper.Map<ReportResponse>(report);
    }

    public async Task<bool> UpdateAsync(Guid id, ReportUpdateRequest request)
    {
        var report = await _context.Reports.FindAsync(id);
        if (report is null) return false;

        _mapper.Map(request, report);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var report = await _context.Reports.FindAsync(id);
        if (report is null) return false;

        _context.Reports.Remove(report);
        await _context.SaveChangesAsync();

        return true;
    }
}
