



namespace LabMS.Services;

public class DoctorService(LabMSDbContext context, IMapper mapper) : IDoctorService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<DoctorResponse>> GetAllAsync()
    {
        var doctors = await _context.Doctors.AsNoTracking().ToListAsync();
        return _mapper.Map<IEnumerable<DoctorResponse>>(doctors);
    }

    public async Task<DoctorResponse?> GetByIdAsync(Guid id)
    {
        var doctor = await _context.Doctors.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);

        return doctor is null ? null : _mapper.Map<DoctorResponse>(doctor);
    }

    public async Task<DoctorResponse> AddAsync(DoctorCreateRequest request)
    {
        var doctor = _mapper.Map<Doctor>(request);

        _context.Doctors.Add(doctor);
        await _context.SaveChangesAsync();

        return _mapper.Map<DoctorResponse>(doctor);
    }

    public async Task<bool> UpdateAsync([FromRoute]Guid id, [FromBody]DoctorUpdateRequest request)
    {
        if (id != request.Id) return false;

        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor is null) return false;

        _mapper.Map(request, doctor);

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var doctor = await _context.Doctors.FindAsync(id);
        if (doctor is null) return false;

        _context.Doctors.Remove(doctor);
        await _context.SaveChangesAsync();
        return true;
    }
}
