namespace LabMS.Services;
public class InvoiceService(LabMSDbContext context, IMapper mapper) : IInvoiceService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<InvoiceResponse?> AddInvoiceForVisitAsync(Guid visitId, decimal discount = 0)
    {
        var visit = await _context.Visits
            .Include(v => v.TestOrders)
            .ThenInclude(to => to.LabTest)
            .Include(v => v.Invoice)
            .FirstOrDefaultAsync(v => v.Id == visitId);

        if (visit is null) return null;
        if (!visit.TestOrders.Any()) return null;
        if (visit.Invoice is not null) return _mapper.Map<InvoiceResponse>(visit.Invoice); // already generated

        var total = visit.TestOrders.Sum(to => to.LabTest.Price);

        var invoice = new Invoice
        {
            Id = Guid.NewGuid(),
            VisitId = visit.Id,
            TotalAmount = total,
            Discount = discount,
            Status = "Unpaid"
        };

        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();

        return _mapper.Map<InvoiceResponse>(invoice);
    }

    public async Task<IEnumerable<InvoiceResponse>> GetAllAsync()
    {
        var invoices = await _context.Invoices
            .Include(i => i.Payments)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<InvoiceResponse>>(invoices);
    }

    public async Task<InvoiceResponse?> GetByIdAsync(Guid id)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Payments)
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == id);

        return invoice is null ? null : _mapper.Map<InvoiceResponse>(invoice);
    }

    public async Task<bool> UpdateAsync(Guid id, InvoiceUpdateRequest request)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice is null) return false;

        _mapper.Map(request, invoice);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var invoice = await _context.Invoices.FindAsync(id);
        if (invoice is null) return false;

        _context.Invoices.Remove(invoice);
        await _context.SaveChangesAsync();

        return true;
    }
}