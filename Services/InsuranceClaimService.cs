namespace LabMS.Services;

public class InsuranceClaimService(LabMSDbContext context, IMapper mapper) : IInsuranceClaimService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<InsuranceClaimResponse?> CreateAsync(InsuranceClaimCreateRequest request, Guid submittedByUserId)
    {
        var claimNumber = await GenerateClaimNumberAsync();

        var claim = new InsuranceClaim
        {
            ClaimNumber = claimNumber,
            InvoiceId = request.InvoiceId,
            PatientId = request.PatientId,
            InsuranceProvider = request.InsuranceProvider,
            PolicyNumber = request.PolicyNumber,
            GroupNumber = request.GroupNumber,
            ClaimAmount = request.ClaimAmount,
            Status = ClaimStatus.Draft,
            SubmittedAt = DateTime.UtcNow,
            SubmittedByUserId = submittedByUserId,
            Notes = request.Notes
        };

        _context.InsuranceClaims.Add(claim);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(claim.Id);
    }

    public async Task<InsuranceClaimResponse?> GetByIdAsync(Guid id)
    {
        var claim = await _context.InsuranceClaims
            .Include(c => c.Patient)
            .Include(c => c.SubmittedBy)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (claim is null) return null;

        return _mapper.Map<InsuranceClaimResponse>(claim);
    }

    public async Task<IEnumerable<InsuranceClaimResponse>> GetAllAsync()
    {
        var claims = await _context.InsuranceClaims
            .Include(c => c.Patient)
            .Include(c => c.SubmittedBy)
            .OrderByDescending(c => c.SubmittedAt)
            .AsNoTracking()
            .ToListAsync();

        return claims.Select(_mapper.Map<InsuranceClaimResponse>);
    }

    public async Task<IEnumerable<InsuranceClaimResponse>> GetByPatientIdAsync(Guid patientId)
    {
        var claims = await _context.InsuranceClaims
            .Include(c => c.Patient)
            .Include(c => c.SubmittedBy)
            .Where(c => c.PatientId == patientId)
            .OrderByDescending(c => c.SubmittedAt)
            .AsNoTracking()
            .ToListAsync();

        return claims.Select(_mapper.Map<InsuranceClaimResponse>);
    }

    public async Task<IEnumerable<InsuranceClaimResponse>> GetByStatusAsync(ClaimStatus status)
    {
        var claims = await _context.InsuranceClaims
            .Include(c => c.Patient)
            .Include(c => c.SubmittedBy)
            .Where(c => c.Status == status)
            .OrderByDescending(c => c.SubmittedAt)
            .AsNoTracking()
            .ToListAsync();

        return claims.Select(_mapper.Map<InsuranceClaimResponse>);
    }

    public async Task<bool> UpdateAsync(Guid id, InsuranceClaimUpdateRequest request)
    {
        var claim = await _context.InsuranceClaims.FindAsync(id);
        if (claim is null) return false;

        if (request.Status.HasValue) claim.Status = request.Status.Value;
        if (request.ApprovedAmount.HasValue) claim.ApprovedAmount = request.ApprovedAmount.Value;
        if (request.PatientResponsibility.HasValue) claim.PatientResponsibility = request.PatientResponsibility.Value;
        if (request.RejectionReason != null) claim.RejectionReason = request.RejectionReason;
        if (request.Notes != null) claim.Notes = request.Notes;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SubmitAsync(Guid id)
    {
        var claim = await _context.InsuranceClaims.FindAsync(id);
        if (claim is null || claim.Status != ClaimStatus.Draft) return false;

        claim.Status = ClaimStatus.Submitted;
        claim.SubmittedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ApproveAsync(Guid id, decimal approvedAmount)
    {
        var claim = await _context.InsuranceClaims.FindAsync(id);
        if (claim is null) return false;

        claim.Status = approvedAmount < claim.ClaimAmount ? ClaimStatus.PartiallyApproved : ClaimStatus.Approved;
        claim.ApprovedAmount = approvedAmount;
        claim.PatientResponsibility = claim.ClaimAmount - approvedAmount;
        claim.ProcessedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RejectAsync(Guid id, string reason)
    {
        var claim = await _context.InsuranceClaims.FindAsync(id);
        if (claim is null) return false;

        claim.Status = ClaimStatus.Rejected;
        claim.RejectionReason = reason;
        claim.ProcessedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MarkAsPaidAsync(Guid id)
    {
        var claim = await _context.InsuranceClaims.FindAsync(id);
        if (claim is null) return false;

        claim.Status = ClaimStatus.Paid;
        claim.PaidAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<string> GenerateClaimNumberAsync()
    {
        var date = DateTime.UtcNow;
        var prefix = $"CLM-{date:yyyyMMdd}";
        
        var lastClaim = await _context.InsuranceClaims
            .Where(c => c.ClaimNumber.StartsWith(prefix))
            .OrderByDescending(c => c.ClaimNumber)
            .FirstOrDefaultAsync();

        int sequence = 1;
        if (lastClaim != null)
        {
            var lastSequence = lastClaim.ClaimNumber.Split('-').Last();
            if (int.TryParse(lastSequence, out int num))
            {
                sequence = num + 1;
            }
        }

        return $"{prefix}-{sequence:D4}";
    }

    
}
