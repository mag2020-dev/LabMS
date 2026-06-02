namespace LabMS.Validators;

public class PurchaseOrderCreateRequestValidator : AbstractValidator<PurchaseOrderCreateRequest>
{
    public PurchaseOrderCreateRequestValidator()
    {
        RuleFor(x => x.SupplierId)
            .NotEmpty().WithMessage("Supplier ID is required");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Purchase order must contain at least one item")
            .Must(items => items.Count > 0).WithMessage("Purchase order must contain at least one item");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.InventoryItemId)
                .NotEmpty().WithMessage("Inventory item ID is required");

            item.RuleFor(x => x.QuantityOrdered)
                .GreaterThan(0).WithMessage("Quantity ordered must be greater than 0");

            item.RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Unit price cannot be negative");
        });
    }
}
