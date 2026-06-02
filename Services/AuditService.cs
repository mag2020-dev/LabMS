using Serilog;

namespace LabMS.Services;

public class AuditService : IAuditService
{
    private readonly ILogger<AuditService> _logger;

    public AuditService(ILogger<AuditService> logger)
    {
        _logger = logger;
    }

    public async Task LogAsync(string action, string entityType, Guid entityId, string userId, string? details = null)
    {
        await LogAsync(action, entityType, entityId.ToString(), userId, details);
    }

    public async Task LogAsync(string action, string entityType, string entityId, string userId, string? details = null)
    {
        var auditLog = new
        {
            Timestamp = DateTime.UtcNow,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            UserId = userId,
            Details = details,
            IpAddress = "Unknown", // Would be extracted from HttpContext in real implementation
            UserAgent = "Unknown"  // Would be extracted from HttpContext in real implementation
        };

        _logger.LogInformation("AUDIT: {Action} on {EntityType} {EntityId} by {UserId} - {Details}",
            action, entityType, entityId, userId, details);

        await Task.CompletedTask;
    }
}
