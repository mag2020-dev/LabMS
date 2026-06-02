

namespace LabMS.Services;

public class PatientService(LabMSDbContext context, IMapper mapper) : IPatientService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<PatientResponse>> GetAllAsync()
    {
        var patients = await _context.Patients.AsNoTracking().ToListAsync();
        return _mapper.Map<IEnumerable<PatientResponse>>(patients);
    }

    public async Task<PatientResponse?> GetByIdAsync(Guid id)
    {
        var patient = await _context.Patients.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return patient is null ? null : _mapper.Map<PatientResponse>(patient);
    }

   

    public async Task<PatientResponse> AddAsync(PatientCreateRequest request)
    {
        var patient = _mapper.Map<Patient>(request);

        _context.Patients.Add(patient);
        await _context.SaveChangesAsync();

        return _mapper.Map<PatientResponse>(patient);
    }
    public async Task<bool> UpdateAsync(Guid id, PatientUpdateRequest request)
    {
        if (id != request.Id) return false;

        var patient = await _context.Patients.FindAsync(id);
        if (patient is null) return false;

        _mapper.Map(request, patient);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var patient = await _context.Patients.FindAsync(id);
        if (patient is null) return false;

        _context.Patients.Remove(patient);
        await _context.SaveChangesAsync();
        return true;
    }
}
