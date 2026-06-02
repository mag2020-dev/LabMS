using LabMS.Contracts.TestResult;
using LabMS.Data;
using LabMS.Entities;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace LabMS.Services;

public class TestResultService(LabMSDbContext context, IMapper mapper) : ITestResultService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<TestResultResponse>> GetAllAsync()
    {
        var results = await _context.TestResults
            .Include(r => r.TestOrder)
                .ThenInclude(to => to.LabTest)
            .Include(r => r.VerifiedBy)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<TestResultResponse>>(results);
    }

    public async Task<TestResultResponse?> GetByIdAsync(Guid id)
    {
        var result = await _context.TestResults
            .Include(r => r.TestOrder)
                .ThenInclude(to => to.LabTest)
            .Include(r => r.VerifiedBy)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        return result is null ? null : _mapper.Map<TestResultResponse>(result);
    }

    public async Task<IEnumerable<TestResultResponse>> GetByTestOrderIdAsync(Guid testOrderId)
    {
        var results = await _context.TestResults
            .Where(r => r.TestOrderId == testOrderId)
            .Include(r => r.TestOrder)
                .ThenInclude(to => to.LabTest)
            .Include(r => r.VerifiedBy)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<TestResultResponse>>(results);
    }

    public async Task<TestResultResponse?> AddAsync(TestResultCreateRequest request)
    {
        var order = await _context.TestOrders
            .Include(to => to.LabTest)
            .FirstOrDefaultAsync(to => to.Id == request.TestOrderId);

        if (order is null) return null;

        var result = _mapper.Map<TestResult>(request);
        result.TestName = order.LabTest?.Name ?? "Unknown Test";

        _context.TestResults.Add(result);
        await _context.SaveChangesAsync();

        return _mapper.Map<TestResultResponse>(result);
    }

    public async Task<bool> UpdateAsync(Guid id, TestResultUpdateRequest request)
    {
        var result = await _context.TestResults.FindAsync(id);
        if (result is null) return false;

        _mapper.Map(request, result);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> VerifyAsync(Guid id, TestResultVerifyRequest request)
    {
        var result = await _context.TestResults.FindAsync(id);
        if (result is null) return false;

        var user = await _context.Users.FindAsync(request.VerifiedById);
        if (user is null) return false;

        result.VerifiedById = request.VerifiedById;
        result.ResultDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var result = await _context.TestResults.FindAsync(id);
        if (result is null) return false;

        _context.TestResults.Remove(result);
        await _context.SaveChangesAsync();
        return true;
    }
}
