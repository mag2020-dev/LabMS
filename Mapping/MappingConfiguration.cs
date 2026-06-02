

namespace LabMS.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // ---------------- Patient ----------------
        config.NewConfig<PatientCreateRequest, Patient>()
            .Ignore(dest => dest.Id);

        config.NewConfig<PatientUpdateRequest, Patient>();

        config.NewConfig<Patient, PatientResponse>()
            .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}");

        // ---------------- Doctor ----------------
        config.NewConfig<DoctorCreateRequest, Doctor>()
            .Ignore(dest => dest.Id);

        config.NewConfig<DoctorUpdateRequest, Doctor>();

        config.NewConfig<Doctor, DoctorResponse>()
            .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}");

        // ---------------- Visit ----------------
        config.NewConfig<VisitCreateRequest, Visit>()
        .Ignore(dest => dest.Id);

        config.NewConfig<VisitUpdateRequest, Visit>();

        config.NewConfig<Visit, VisitResponse>()
            .Map(dest => dest.PatientName, src => src.PatientName ?? string.Empty)
            .Map(dest => dest.DoctorName, src => src.DoctorName) 
            .Map(dest => dest.Reason, src => src.Reason)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Notes, src => src.Notes);

        // ---------------- LabTest ----------------

        config.NewConfig<LabTestCreateRequest, LabTest>()
        .Ignore(dest => dest.Id);

        config.NewConfig<LabTestUpdateRequest, LabTest>();

        config.NewConfig<LabTest, LabTestResponse>();

        // ---------------- TestOrder ----------------
        config.NewConfig<TestOrderCreateRequest, TestOrder>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.LabTestId)   // resolved by service
            .Ignore(dest => dest.Visit)       // handled by EF
            .Ignore(dest => dest.LabTest);    // handled by EF

        config.NewConfig<TestOrderUpdateRequest, TestOrder>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.LabTestId)   // service resolves LabTestName
            .Ignore(dest => dest.Visit)
            .Ignore(dest => dest.LabTest);

        config.NewConfig<TestOrder, TestOrderResponse>()
            .Map(dest => dest.LabTestName, src => src.LabTest.Name)
            .Map(dest => dest.VisitId, src => src.VisitId)
            .Map(dest => dest.VisitDate, src => src.Visit.VisitDate)
            .Map(dest => dest.PatientName,
                 src => src.Visit.Patient.FirstName + " " + src.Visit.Patient.LastName)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.OrderedAt, src => src.OrderedAt);


        // ----------------TestResult ----------------
        TypeAdapterConfig<TestResult, TestResultResponse>
              .NewConfig()
              .Map(dest => dest.VerifiedByName, src => src.VerifiedBy != null ? $"{src.VerifiedBy.FullName} " : null);

        // CreateRequest -> Entity
        TypeAdapterConfig<TestResultCreateRequest, TestResult>
            .NewConfig()
            .Map(dest => dest.ResultDate, src => DateTime.UtcNow);

        // UpdateRequest -> Entity
        TypeAdapterConfig<TestResultUpdateRequest, TestResult>
            .NewConfig()
            .IgnoreNullValues(true); // Optional: only update non-null fields

        // VerifyRequest -> Entity
        TypeAdapterConfig<TestResultVerifyRequest, TestResult>
            .NewConfig()
            .Map(dest => dest.VerifiedById, src => src.VerifiedById);

        // ---------------- Report ----------------
        config.NewConfig<ReportCreateRequest, Report>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.GeneratedAt);

        config.NewConfig<ReportUpdateRequest, Report>();

        config.NewConfig<Report, ReportResponse>()
            .Map(dest => dest.GeneratedByName, src => src.GeneratedBy != null
                ? src.GeneratedBy.Username
                : null);
        // ---------------- Invoice ----------------
        config.NewConfig<InvoiceCreateRequest, Invoice>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.IssuedAt);

        config.NewConfig<InvoiceUpdateRequest, Invoice>();

        config.NewConfig<Invoice, InvoiceResponse>();

        // ---------------- Payment ----------------
        config.NewConfig<PaymentCreateRequest, Payment>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.PaymentDate);

        config.NewConfig<PaymentUpdateRequest, Payment>();

        config.NewConfig<Payment, PaymentResponse>();

        // ---------------- User ----------------
        config.NewConfig<UserRegisterRequest, User>()
            .Ignore(dest => dest.Id)
            .Map(dest => dest.PasswordHash, src => BCrypt.Net.BCrypt.HashPassword(src.Password));

        config.NewConfig<User, UserResponse>()
            .Map(dest => dest.Roles,
                 src => src.UserRoles.Select(ur => ur.Role.Name));

        // ---------------- Role ----------------
        config.NewConfig<Role, RoleResponse>();

        // ---------------- TestPackage ----------------
        config.NewConfig<TestPackageUpdateRequest, TestPackage>()
            .IgnoreNullValues(true)
            .Ignore(dest => dest.TestPackageItems)
            .Ignore(dest => dest.RegularPrice)
            .Ignore(dest => dest.DiscountPercentage);

        // ---------------- PackageSubscription ----------------
        config.NewConfig<PackageSubscriptionCreateRequest, PackageSubscription>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.Status)
            .Ignore(dest => dest.MonthlyPrice)
            .Ignore(dest => dest.CreatedAt);

        // ---------------- Sample ----------------
        config.NewConfig<SampleUpdateRequest, Sample>()
            .IgnoreNullValues(true);

        // ================= INVENTORY =================
        // InventoryItem -> InventoryItemResponse
        config.NewConfig<InventoryItem, InventoryItemResponse>()
            .Map(dest => dest.IsLowStock, src => src.Quantity <= src.ReorderLevel)
            .Map(dest => dest.IsExpiringSoon, src => src.ExpiryDate != null && src.ExpiryDate <= DateTime.UtcNow.AddDays(30));

        // InventoryTransaction -> InventoryTransactionResponse
        config.NewConfig<InventoryTransaction, InventoryTransactionResponse>()
            .Map(dest => dest.ItemName, src => src.InventoryItem.Name)
            .Map(dest => dest.UserName, src => src.User != null ? src.User.Username : null)
            .Map(dest => dest.SupplierName, src => src.Supplier != null ? src.Supplier.Name : null);

        // InventoryAlert -> InventoryAlertResponse
        config.NewConfig<InventoryAlert, InventoryAlertResponse>()
            .Map(dest => dest.ItemName, src => src.InventoryItem.Name)
            .Map(dest => dest.ResolvedByName, src => src.ResolvedBy != null ? src.ResolvedBy.Username : null);

        // Supplier -> SupplierResponse
        config.NewConfig<Supplier, SupplierResponse>();

        // PurchaseOrderItem -> PurchaseOrderItemResponse
        config.NewConfig<PurchaseOrderItem, PurchaseOrderItemResponse>()
            .Map(dest => dest.ItemName, src => src.InventoryItem.Name);

        // PurchaseOrder -> PurchaseOrderResponse
        config.NewConfig<PurchaseOrder, PurchaseOrderResponse>()
            .Map(dest => dest.SupplierName, src => src.Supplier.Name)
            .Map(dest => dest.CreatedByName, src => src.CreatedBy.Username)
            .Map(dest => dest.ApprovedByName, src => src.ApprovedBy != null ? src.ApprovedBy.Username : null)
            .Map(dest => dest.Items, src => src.Items);

        // ================= BILLING =================
        // DiscountRule -> DiscountRuleResponse
        config.NewConfig<DiscountRule, DiscountRuleResponse>();

        // Refund -> RefundResponse
        config.NewConfig<Refund, RefundResponse>()
            .Map(dest => dest.RequestedByName, src => src.RequestedBy.Username)
            .Map(dest => dest.ApprovedByName, src => src.ApprovedBy != null ? src.ApprovedBy.Username : null);

        // TaxConfiguration -> TaxConfigurationResponse
        config.NewConfig<TaxConfiguration, TaxConfigurationResponse>();

        // InsuranceClaim -> InsuranceClaimResponse
        config.NewConfig<InsuranceClaim, InsuranceClaimResponse>()
            .Map(dest => dest.PatientName, src => src.Patient.FirstName + " " + src.Patient.LastName)
            .Map(dest => dest.SubmittedByName, src => src.SubmittedBy.Username);

        // PaymentPlanInstallment -> PaymentPlanInstallmentResponse
        config.NewConfig<PaymentPlanInstallment, PaymentPlanInstallmentResponse>();

        // PaymentPlan -> PaymentPlanResponse
        config.NewConfig<PaymentPlan, PaymentPlanResponse>()
            .Map(dest => dest.PatientName, src => src.Patient.FirstName + " " + src.Patient.LastName)
            .Map(dest => dest.Installments, src => src.Installments);
    }




}
