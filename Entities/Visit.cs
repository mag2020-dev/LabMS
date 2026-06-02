namespace LabMS.Entities;

public class Visit
{
    public Guid Id { get; set; }

  
    public Guid PatientId { get; set; }
    public Patient Patient { get; set; } = default!;

    public Guid? DoctorId { get; set; }
    public Doctor? Doctor { get; set; }


    public string PatientName { get; set; } = default!;
    public string? DoctorName { get; set; }

    public DateTime VisitDate { get; set; }
    public string? Reason { get; set; }
    public VisitStatus Status { get; set; } = VisitStatus.Pending;
    public string? Notes { get; set; }

    
    public ICollection<TestOrder> TestOrders { get; set; } = [];
    public Report? Report { get; set; }
    public Invoice? Invoice { get; set; }
}

public enum VisitStatus
{
    Pending,
    InProgress,
    Completed,
    Cancelled
}
