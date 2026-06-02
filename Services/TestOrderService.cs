using LabMS.Contracts.TestOrder;
using LabMS.Data;
using LabMS.Entities;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace LabMS.Services;

public class TestOrderService(LabMSDbContext context, IMapper mapper) : ITestOrderService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<TestOrderResponse>> GetAllAsync()
    {
        var orders = await _context.TestOrders
            .Include(o => o.LabTest)
            .Include(o => o.Visit)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<TestOrderResponse>>(orders);
    }

    public async Task<TestOrderResponse?> GetByIdAsync(Guid id)
    {
        var order = await _context.TestOrders
            .Include(o => o.LabTest)
            .Include(o => o.Visit)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);

        return order is null ? null : _mapper.Map<TestOrderResponse>(order);
    }

    public async Task<TestOrderResponse?> AddAsync(TestOrderCreateRequest request)
    {
       
        var labTest = await _context.LabTests.FirstOrDefaultAsync(t => t.Name == request.LabTestName);
        if (labTest is null) return null;

    
        var visit = await _context.Visits.Include(v => v.Invoice)
            .FirstOrDefaultAsync(v => v.Id == request.VisitId);
        if (visit is null) return null;

        
        if (visit.Invoice is null || visit.Invoice.Status != "Paid")
            throw new InvalidOperationException("Cannot order tests until invoice is paid.");

        var order = _mapper.Map<TestOrder>(request);
        order.LabTestId = labTest.Id;

        _context.TestOrders.Add(order);
        await _context.SaveChangesAsync();

        return _mapper.Map<TestOrderResponse>(order);
    }

    
    public async Task<bool> UpdateAsync(Guid id, TestOrderUpdateRequest request)
    {
        var order = await _context.TestOrders.FindAsync(id);
        if (order is null) return false;

        _mapper.Map(request, order);
        await _context.SaveChangesAsync();
        return true;
    }

    
    public async Task<bool> UpdateStatusAsync(Guid id, string newStatus)
    {
        var order = await _context.TestOrders
            .Include(o => o.Visit)
                .ThenInclude(v => v.Invoice)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null) return false;

        if (newStatus == "InProgress")
        {
            if (order.Visit?.Invoice?.Status != "Paid")
                throw new InvalidOperationException("Cannot start test unless invoice is paid.");
        }

        order.Status = newStatus;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var order = await _context.TestOrders.FindAsync(id);
        if (order is null) return false;

        _context.TestOrders.Remove(order);
        await _context.SaveChangesAsync();
        return true;
    }
}
