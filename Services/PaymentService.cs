
namespace LabMS.Services;
public class PaymentService(LabMSDbContext context, IMapper mapper, ITransactionService transactionService) : IPaymentService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;
    private readonly ITransactionService _transactionService = transactionService;

    public async Task<PaymentResponse?> AddPaymentAsync(PaymentCreateRequest request)
    {
        return await _transactionService.ExecuteInTransactionAsync(async () =>
        {
            var invoice = await _context.Invoices
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == request.InvoiceId);

            if (invoice is null) return null;

            var payment = _mapper.Map<Payment>(request);
            invoice.Payments.Add(payment);

            UpdateInvoiceStatus(invoice);

            await _context.SaveChangesAsync();
            return _mapper.Map<PaymentResponse>(payment);
        });
    }

    public async Task<IEnumerable<PaymentResponse>> GetByInvoiceIdAsync(Guid invoiceId)
    {
        var payments = await _context.Payments
            .Where(p => p.InvoiceId == invoiceId)
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<PaymentResponse>>(payments);
    }

    public async Task<PaymentResponse?> GetByIdAsync(Guid id)
    {
        var payment = await _context.Payments.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        return payment is null ? null : _mapper.Map<PaymentResponse>(payment);
    }

    public async Task<IEnumerable<PaymentResponse>> GetAllAsync()
    {
        var payments = await _context.Payments.AsNoTracking().ToListAsync();
        return _mapper.Map<IEnumerable<PaymentResponse>>(payments);
    }

    public async Task<bool> UpdateAsync(Guid id, PaymentUpdateRequest request)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment is null) return false;

        _mapper.Map(request, payment);

        // Ensure invoice status is recalculated
        var invoice = await _context.Invoices
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == payment.InvoiceId);

        if (invoice is not null)
            UpdateInvoiceStatus(invoice);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var payment = await _context.Payments.FindAsync(id);
        if (payment is null) return false;

        var invoiceId = payment.InvoiceId;

        _context.Payments.Remove(payment);

        // Recalculate invoice after deletion
        var invoice = await _context.Invoices
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == invoiceId);

        if (invoice is not null)
            UpdateInvoiceStatus(invoice);

        await _context.SaveChangesAsync();
        return true;
    }

    private static void UpdateInvoiceStatus(Invoice invoice)
    {
        var totalPaid = invoice.Payments.Sum(p => p.Amount);
        var due = invoice.TotalAmount - invoice.Discount;

        if (totalPaid >= due)
            invoice.Status = "Paid";
        else if (totalPaid > 0)
            invoice.Status = "Partial";
        else
            invoice.Status = "Unpaid";
    }
}
