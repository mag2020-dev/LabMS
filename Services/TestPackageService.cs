namespace LabMS.Services;

public class TestPackageService(LabMSDbContext context, IMapper mapper) : ITestPackageService
{
    private readonly LabMSDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public async Task<TestPackageResponse?> CreateAsync(TestPackageCreateRequest request)
    {
        var package = new TestPackage
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            PackagePrice = request.PackagePrice,
            Type = request.Type,
            IsActive = true
        };

        // Add test items
        foreach (var testItem in request.Tests)
        {
            var labTest = await _context.LabTests.FindAsync(testItem.LabTestId);
            if (labTest is null) continue;

            package.TestPackageItems.Add(new TestPackageItem
            {
                LabTestId = testItem.LabTestId,
                Quantity = testItem.Quantity,
                IsOptional = testItem.IsOptional
            });
        }

        // Calculate regular price and discount
        var regularPrice = await CalculateRegularPriceAsync(package.TestPackageItems);
        package.RegularPrice = regularPrice;
        package.DiscountPercentage = regularPrice > 0 
            ? Math.Round((regularPrice - request.PackagePrice) / regularPrice * 100, 2) 
            : 0;

        _context.TestPackages.Add(package);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(package.Id);
    }

    public async Task<TestPackageResponse?> GetByIdAsync(Guid id)
    {
        var package = await _context.TestPackages
            .Include(p => p.TestPackageItems)
                .ThenInclude(i => i.LabTest)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (package is null) return null;

        return MapToResponse(package);
    }

    public async Task<IEnumerable<TestPackageResponse>> GetAllAsync()
    {
        var packages = await _context.TestPackages
            .Include(p => p.TestPackageItems)
                .ThenInclude(i => i.LabTest)
            .OrderBy(p => p.Name)
            .AsNoTracking()
            .ToListAsync();

        return packages.Select(MapToResponse);
    }

    public async Task<IEnumerable<TestPackageResponse>> GetActiveAsync()
    {
        var packages = await _context.TestPackages
            .Include(p => p.TestPackageItems)
                .ThenInclude(i => i.LabTest)
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .AsNoTracking()
            .ToListAsync();

        return packages.Select(MapToResponse);
    }

    public async Task<IEnumerable<TestPackageResponse>> GetByTypeAsync(PackageType type)
    {
        var packages = await _context.TestPackages
            .Include(p => p.TestPackageItems)
                .ThenInclude(i => i.LabTest)
            .Where(p => p.Type == type && p.IsActive)
            .OrderBy(p => p.Name)
            .AsNoTracking()
            .ToListAsync();

        return packages.Select(MapToResponse);
    }

    public async Task<bool> UpdateAsync(Guid id, TestPackageUpdateRequest request)
    {
        var package = await _context.TestPackages
            .Include(p => p.TestPackageItems)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (package is null) return false;

        // Update basic properties
        if (request.Name != null) package.Name = request.Name;
        if (request.Code != null) package.Code = request.Code;
        if (request.Description != null) package.Description = request.Description;
        if (request.PackagePrice.HasValue) package.PackagePrice = request.PackagePrice.Value;
        if (request.Type.HasValue) package.Type = request.Type.Value;
        if (request.IsActive.HasValue) package.IsActive = request.IsActive.Value;

        // Update tests if provided
        if (request.Tests != null)
        {
            // Remove existing items
            _context.TestPackageItems.RemoveRange(package.TestPackageItems);

            // Add new items
            foreach (var testItem in request.Tests)
            {
                package.TestPackageItems.Add(new TestPackageItem
                {
                    TestPackageId = package.Id,
                    LabTestId = testItem.LabTestId,
                    Quantity = testItem.Quantity,
                    IsOptional = testItem.IsOptional
                });
            }

            // Recalculate prices
            var regularPrice = await CalculateRegularPriceAsync(package.TestPackageItems);
            package.RegularPrice = regularPrice;
            package.DiscountPercentage = regularPrice > 0
                ? Math.Round((regularPrice - package.PackagePrice) / regularPrice * 100, 2)
                : 0;
        }

        package.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var package = await _context.TestPackages.FindAsync(id);
        if (package is null) return false;

        _context.TestPackages.Remove(package);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ActivateAsync(Guid id)
    {
        var package = await _context.TestPackages.FindAsync(id);
        if (package is null) return false;

        package.IsActive = true;
        package.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeactivateAsync(Guid id)
    {
        var package = await _context.TestPackages.FindAsync(id);
        if (package is null) return false;

        package.IsActive = false;
        package.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<decimal> CalculateRegularPriceAsync(ICollection<TestPackageItem> items)
    {
        decimal total = 0;
        foreach (var item in items)
        {
            var labTest = await _context.LabTests.FindAsync(item.LabTestId);
            if (labTest != null)
            {
                total += labTest.Price * item.Quantity;
            }
        }
        return total;
    }

    private static TestPackageResponse MapToResponse(TestPackage package)
    {
        var testItems = package.TestPackageItems.Select(i => new TestPackageItemResponse(
            i.Id,
            i.LabTestId,
            i.LabTest.Name,
            i.LabTest.Price,
            i.Quantity,
            i.IsOptional
        )).ToList();

        var savings = package.RegularPrice - package.PackagePrice;

        return new TestPackageResponse(
            package.Id,
            package.Name,
            package.Code,
            package.Description,
            package.RegularPrice,
            package.PackagePrice,
            package.DiscountPercentage,
            savings,
            package.IsActive,
            package.Type,
            testItems,
            package.CreatedAt
        );
    }
}
