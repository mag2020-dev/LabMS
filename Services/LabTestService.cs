

namespace LabMS.Services;

public class LabTestService(LabMSDbContext context, IMapper mapper) : ILabTestService
    {
        private readonly LabMSDbContext _context = context;
        private readonly IMapper _mapper = mapper;

    public async Task<IEnumerable<LabTestResponse>> GetAllAsync()
    {
        var labTests = await _context.Patients.AsNoTracking().ToListAsync();
        return _mapper.Map<IEnumerable<LabTestResponse>>(labTests);
    }

    public async Task<LabTestResponse?> GetByIdAsync(Guid id)
    {
        var labTest = await _context.LabTests.AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return labTest is null ? null : _mapper.Map<LabTestResponse>(labTest);
    }

    public async Task<LabTestResponse> AddAsync(LabTestCreateRequest request)
        {
            var labTest = _mapper.Map<LabTest>(request);

            _context.LabTests.Add(labTest);
            await _context.SaveChangesAsync();

            return _mapper.Map<LabTestResponse>(labTest);
        }

        public async Task<bool> UpdateAsync(Guid id, LabTestUpdateRequest request)
        {
            

            var labTest = await _context.LabTests.FindAsync(id);
            if (labTest is null) return false;

            _mapper.Map(request, labTest);
            await _context.SaveChangesAsync();

            return true;
        }
        

        public async Task<bool> DeleteAsync(Guid id)
        {
            var labTest = await _context.LabTests.FindAsync(id);
            if (labTest is null) return false;
        
            _context.LabTests.Remove(labTest);
            await _context.SaveChangesAsync();
           
            return true;
        }

   
}



