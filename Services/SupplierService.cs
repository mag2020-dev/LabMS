namespace LabMS.Services;

public class SupplierService(LabMSDbContext context, IMapper mapper) : ISupplierService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<SupplierResponse?> CreateAsync(SupplierCreateRequest request)
    {
        var supplier = new Supplier
        {
            Name = request.Name,
            Code = request.Code,
            ContactPerson = request.ContactPerson,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address,
            IsActive = true
        };

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(supplier.Id);
    }

    public async Task<SupplierResponse?> GetByIdAsync(Guid id)
    {
        var supplier = await _context.Suppliers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);

        if (supplier is null) return null;

        return _mapper.Map<SupplierResponse>(supplier);
    }

    public async Task<IEnumerable<SupplierResponse>> GetAllAsync()
    {
        var suppliers = await _context.Suppliers
            .OrderBy(s => s.Name)
            .AsNoTracking()
            .ToListAsync();

        return suppliers.Select(_mapper.Map<SupplierResponse>);
    }

    public async Task<IEnumerable<SupplierResponse>> GetActiveAsync()
    {
        var suppliers = await _context.Suppliers
            .Where(s => s.IsActive)
            .OrderBy(s => s.Name)
            .AsNoTracking()
            .ToListAsync();

        return suppliers.Select(_mapper.Map<SupplierResponse>);
    }

    public async Task<bool> UpdateAsync(Guid id, SupplierUpdateRequest request)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier is null) return false;

        if (request.Name != null) supplier.Name = request.Name;
        if (request.ContactPerson != null) supplier.ContactPerson = request.ContactPerson;
        if (request.Email != null) supplier.Email = request.Email;
        if (request.Phone != null) supplier.Phone = request.Phone;
        if (request.Address != null) supplier.Address = request.Address;
        if (request.IsActive.HasValue) supplier.IsActive = request.IsActive.Value;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var supplier = await _context.Suppliers.FindAsync(id);
        if (supplier is null) return false;

        _context.Suppliers.Remove(supplier);
        await _context.SaveChangesAsync();
        return true;
    }
}
