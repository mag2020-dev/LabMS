using LabMS.Contracts.Visit;
using LabMS.Data;
using LabMS.Entities;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace LabMS.Services;

public class VisitService(LabMSDbContext context, IMapper mapper) : IVisitService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<VisitResponse>> GetAllAsync()
    {
        var visits = await _context.Visits
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<VisitResponse>>(visits);
    }

    public async Task<VisitResponse?> GetByIdAsync(Guid id)
    {
        var visit = await _context.Visits
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Id == id);

        return visit is null ? null : _mapper.Map<VisitResponse>(visit);
    }

    public async Task<VisitResponse> AddAsync(VisitCreateRequest request)
    {
        // Find Patient
        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.FirstName + " " + p.LastName == request.PatientName);

        if (patient is null)
            throw new InvalidOperationException($"Patient '{request.PatientName}' not found.");

        // Find Doctor (optional)
        Doctor? doctor = null;
        if (!string.IsNullOrWhiteSpace(request.DoctorName))
        {
            doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.FirstName + " " + d.LastName == request.DoctorName);

            if (doctor is null)
                throw new InvalidOperationException($"Doctor '{request.DoctorName}' not found.");
        }

        var visit = new Visit
        {
            Id = Guid.NewGuid(),
            PatientId = patient.Id,
            PatientName = request.PatientName,
            DoctorId = doctor?.Id,
            DoctorName = doctor is null ? null : request.DoctorName,
            VisitDate = request.VisitDate,
            Reason = request.Reason,
            Status = VisitStatus.Pending // always default
        };

        _context.Visits.Add(visit);
        await _context.SaveChangesAsync();

        return _mapper.Map<VisitResponse>(visit);
    }

    public async Task<bool> UpdateAsync(Guid id, VisitUpdateRequest request)
    {
        var visit = await _context.Visits.FindAsync(id);
        if (visit is null)
            return false;

        // Update Doctor (optional)
        if (!string.IsNullOrWhiteSpace(request.DoctorName))
        {
            var doctor = await _context.Doctors
                .FirstOrDefaultAsync(d => d.FirstName + " " + d.LastName == request.DoctorName);

            if (doctor is null)
                throw new InvalidOperationException($"Doctor '{request.DoctorName}' not found.");

            visit.DoctorId = doctor.Id;
            visit.DoctorName = request.DoctorName;
        }

        visit.VisitDate = request.VisitDate;
        visit.Reason = request.Reason;
        visit.Status = request.Status;
        visit.Notes = request.Notes;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var visit = await _context.Visits.FindAsync(id);
        if (visit is null) return false;
        _context.Visits.Remove(visit);
        await _context.SaveChangesAsync();
        return true;
    }
}
