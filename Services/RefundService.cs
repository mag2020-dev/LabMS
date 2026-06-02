namespace LabMS.Services;

public class RefundService(LabMSDbContext context, IMapper mapper) : IRefundService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<RefundResponse?> CreateAsync(RefundCreateRequest request, Guid requestedByUserId)
    {
        var refundNumber = await GenerateRefundNumberAsync();

        var refund = new Refund
        {
            RefundNumber = refundNumber,
            PaymentId = request.PaymentId,
            Amount = request.Amount,
            Reason = request.Reason,
            Method = request.Method,
            Status = RefundStatus.Pending,
            RequestedByUserId = requestedByUserId,
            Notes = request.Notes
        };

        _context.Refunds.Add(refund);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(refund.Id);
    }

    public async Task<RefundResponse?> GetByIdAsync(Guid id)
    {
        var refund = await _context.Refunds
            .Include(r => r.Payment)
            .Include(r => r.RequestedBy)
            .Include(r => r.ApprovedBy)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id);

        if (refund is null) return null;

        return _mapper.Map<RefundResponse>(refund);
    }

    public async Task<IEnumerable<RefundResponse>> GetAllAsync()
    {
        var refunds = await _context.Refunds
            .Include(r => r.Payment)
            .Include(r => r.RequestedBy)
            .Include(r => r.ApprovedBy)
            .OrderByDescending(r => r.RequestedAt)
            .AsNoTracking()
            .ToListAsync();

        return refunds.Select(_mapper.Map<RefundResponse>);
    }

    public async Task<IEnumerable<RefundResponse>> GetByStatusAsync(RefundStatus status)
    {
        var refunds = await _context.Refunds
            .Include(r => r.Payment)
            .Include(r => r.RequestedBy)
            .Include(r => r.ApprovedBy)
            .Where(r => r.Status == status)
            .OrderByDescending(r => r.RequestedAt)
            .AsNoTracking()
            .ToListAsync();

        return refunds.Select(_mapper.Map<RefundResponse>);
    }

    public async Task<bool> ApproveAsync(Guid id, Guid approvedByUserId, string? notes = null)
    {
        var refund = await _context.Refunds.FindAsync(id);
        if (refund is null || refund.Status != RefundStatus.Pending) return false;

        refund.Status = RefundStatus.Approved;
        refund.ApprovedByUserId = approvedByUserId;
        refund.ApprovedAt = DateTime.UtcNow;
        if (notes != null) refund.Notes = notes;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectAsync(Guid id, Guid approvedByUserId, string? notes = null)
    {
        var refund = await _context.Refunds.FindAsync(id);
        if (refund is null || refund.Status != RefundStatus.Pending) return false;

        refund.Status = RefundStatus.Rejected;
        refund.ApprovedByUserId = approvedByUserId;
        refund.ApprovedAt = DateTime.UtcNow;
        if (notes != null) refund.Notes = notes;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ProcessAsync(Guid id)
    {
        var refund = await _context.Refunds.FindAsync(id);
        if (refund is null || refund.Status != RefundStatus.Approved) return false;

        refund.Status = RefundStatus.Processed;
        refund.ProcessedAt = DateTime.UtcNow;

        // Here you would integrate with a payment gateway to process the refund

        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<string> GenerateRefundNumberAsync()
    {
        var date = DateTime.UtcNow;
        var prefix = $"REF-{date:yyyyMMdd}";
        
        var lastRefund = await _context.Refunds
            .Where(r => r.RefundNumber.StartsWith(prefix))
            .OrderByDescending(r => r.RefundNumber)
            .FirstOrDefaultAsync();

        int sequence = 1;
        if (lastRefund != null)
        {
            var lastSequence = lastRefund.RefundNumber.Split('-').Last();
            if (int.TryParse(lastSequence, out int num))
            {
                sequence = num + 1;
            }
        }

        return $"{prefix}-{sequence:D4}";
    }

    
}
