namespace LabMS.Entities;

public class SampleTracking
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid SampleId { get; set; }
    public Sample Sample { get; set; } = default!;

    public SampleStatus Status { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public Guid? UserId { get; set; }
    public User? User { get; set; }

    public string? Location { get; set; }
    public string? Notes { get; set; }
}
