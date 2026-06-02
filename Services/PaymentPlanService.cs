namespace LabMS.Services;

public class PaymentPlanService(LabMSDbContext context, IMapper mapper) : IPaymentPlanService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<PaymentPlanResponse?> CreateAsync(PaymentPlanCreateRequest request, Guid createdByUserId)
    {
        var invoice = await _context.Invoices.FindAsync(request.InvoiceId);
        if (invoice is null) return null;

        var remainingAmount = invoice.TotalAmount - invoice.Discount - request.DownPayment;
        var installmentAmount = remainingAmount / request.NumberOfInstallments;

        var plan = new PaymentPlan
        {
            InvoiceId = request.InvoiceId,
            PatientId = request.PatientId,
            TotalAmount = invoice.TotalAmount - invoice.Discount,
            DownPayment = request.DownPayment,
            InstallmentAmount = installmentAmount,
            NumberOfInstallments = request.NumberOfInstallments,
            Frequency = request.Frequency,
            StartDate = request.StartDate,
            CreatedByUserId = createdByUserId,
            Status = PaymentPlanStatus.Active
        };

        // Create installments
        for (int i = 1; i <= request.NumberOfInstallments; i++)
        {
            plan.Installments.Add(new PaymentPlanInstallment
            {
                InstallmentNumber = i,
                Amount = installmentAmount,
                DueDate = CalculateNextDueDate(request.StartDate, request.Frequency, i - 1)
            });
        }

        plan.NextPaymentDate = plan.Installments.First().DueDate;

        _context.PaymentPlans.Add(plan);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(plan.Id);
    }

    public async Task<PaymentPlanResponse?> GetByIdAsync(Guid id)
    {
        var plan = await _context.PaymentPlans
            .Include(p => p.Patient)
            .Include(p => p.Installments)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (plan is null) return null;

        return _mapper.Map<PaymentPlanResponse>(plan);
    }

    public async Task<IEnumerable<PaymentPlanResponse>> GetAllAsync()
    {
        var plans = await _context.PaymentPlans
            .Include(p => p.Patient)
            .Include(p => p.Installments)
            .OrderByDescending(p => p.CreatedAt)
            .AsNoTracking()
            .ToListAsync();

        return plans.Select(_mapper.Map<PaymentPlanResponse>);
    }

    public async Task<IEnumerable<PaymentPlanResponse>> GetByPatientIdAsync(Guid patientId)
    {
        var plans = await _context.PaymentPlans
            .Include(p => p.Patient)
            .Include(p => p.Installments)
            .Where(p => p.PatientId == patientId)
            .OrderByDescending(p => p.CreatedAt)
            .AsNoTracking()
            .ToListAsync();

        return plans.Select(_mapper.Map<PaymentPlanResponse>);
    }

    public async Task<IEnumerable<PaymentPlanResponse>> GetActiveAsync()
    {
        var plans = await _context.PaymentPlans
            .Include(p => p.Patient)
            .Include(p => p.Installments)
            .Where(p => p.Status == PaymentPlanStatus.Active)
            .OrderBy(p => p.NextPaymentDate)
            .AsNoTracking()
            .ToListAsync();

        return plans.Select(_mapper.Map<PaymentPlanResponse>);
    }

    public async Task<bool> ProcessPaymentAsync(Guid planId, PaymentPlanPaymentRequest request)
    {
        var plan = await _context.PaymentPlans
            .Include(p => p.Installments)
            .FirstOrDefaultAsync(p => p.Id == planId);

        if (plan is null) return false;

        var installment = plan.Installments.FirstOrDefault(i => i.Id == request.InstallmentId && !i.IsPaid);
        if (installment is null) return false;

        // Create payment record
        var payment = new Payment
        {
            InvoiceId = plan.InvoiceId,
            Amount = request.Amount,
            Method = request.PaymentMethod,
            Reference = request.Reference,
            PaymentDate = DateTime.UtcNow
        };

        _context.Payments.Add(payment);

        // Update installment
        installment.IsPaid = true;
        installment.PaidDate = DateTime.UtcNow;
        installment.AmountPaid = request.Amount;
        installment.PaymentId = payment.Id;

        plan.InstallmentsPaid++;

        // Update plan status
        if (plan.InstallmentsPaid >= plan.NumberOfInstallments)
        {
            plan.Status = PaymentPlanStatus.Completed;
            plan.NextPaymentDate = null;
        }
        else
        {
            plan.NextPaymentDate = plan.Installments
                .Where(i => !i.IsPaid)
                .OrderBy(i => i.DueDate)
                .First().DueDate;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelAsync(Guid id)
    {
        var plan = await _context.PaymentPlans.FindAsync(id);
        if (plan is null) return false;

        plan.Status = PaymentPlanStatus.Cancelled;
        await _context.SaveChangesAsync();
        return true;
    }

    private static DateTime CalculateNextDueDate(DateTime startDate, PaymentFrequency frequency, int offset)
    {
        return frequency switch
        {
            PaymentFrequency.Weekly => startDate.AddDays(7 * offset),
            PaymentFrequency.BiWeekly => startDate.AddDays(14 * offset),
            PaymentFrequency.Monthly => startDate.AddMonths(offset),
            PaymentFrequency.Quarterly => startDate.AddMonths(3 * offset),
            _ => startDate
        };
    }

    
}
