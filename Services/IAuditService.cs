namespace LabMS.Services;

public interface IAuditService
{
    Task LogAsync(string action, string entityType, Guid entityId, string userId, string? details = null);
    Task LogAsync(string action, string entityType, string entityId, string userId, string? details = null);
}
