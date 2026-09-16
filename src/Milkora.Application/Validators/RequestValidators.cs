using FluentValidation;
using Milkora.Application.DTOs;
using Milkora.Domain.Constants;

namespace Milkora.Application.Validators;

// One validator per request DTO. Registered automatically by assembly scan
// (see Application DependencyInjection) and executed by the API's ValidationFilter.

public sealed class CreateAnimalRequestValidator : AbstractValidator<CreateAnimalRequest>
{
    public CreateAnimalRequestValidator()
    {
        RuleFor(x => x.TagNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).MaximumLength(100);
        RuleFor(x => x.Status).NotEmpty().Must(s => AnimalStatuses.All.Contains(s))
            .WithMessage($"Status must be one of: {string.Join(", ", AnimalStatuses.All)}.");
        RuleFor(x => x.PurchasePrice).GreaterThanOrEqualTo(0).When(x => x.PurchasePrice.HasValue);
        RuleFor(x => x.DailyMilkYield).GreaterThanOrEqualTo(0).When(x => x.DailyMilkYield.HasValue);
    }
}

public sealed class UpdateAnimalRequestValidator : AbstractValidator<UpdateAnimalRequest>
{
    public UpdateAnimalRequestValidator()
    {
        RuleFor(x => x.TagNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Status).NotEmpty().Must(s => AnimalStatuses.All.Contains(s))
            .WithMessage($"Status must be one of: {string.Join(", ", AnimalStatuses.All)}.");
        RuleFor(x => x.PurchasePrice).GreaterThanOrEqualTo(0).When(x => x.PurchasePrice.HasValue);
        RuleFor(x => x.DailyMilkYield).GreaterThanOrEqualTo(0).When(x => x.DailyMilkYield.HasValue);
    }
}

public sealed class CreateMilkLogRequestValidator : AbstractValidator<CreateMilkLogRequest>
{
    public CreateMilkLogRequestValidator()
    {
        RuleFor(x => x.LogDate).NotEqual(default(DateTime)).WithMessage("LogDate is required.");
        RuleFor(x => x.MorningMilk).GreaterThanOrEqualTo(0);
        RuleFor(x => x.EveningMilk).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FatPercent).InclusiveBetween(0, 100).When(x => x.FatPercent.HasValue);
        RuleFor(x => x.RatePerLitre).GreaterThanOrEqualTo(0).When(x => x.RatePerLitre.HasValue);
    }
}

public sealed class UpdateMilkLogRequestValidator : AbstractValidator<UpdateMilkLogRequest>
{
    public UpdateMilkLogRequestValidator()
    {
        RuleFor(x => x.LogDate).NotEqual(default(DateTime)).WithMessage("LogDate is required.");
        RuleFor(x => x.MorningMilk).GreaterThanOrEqualTo(0);
        RuleFor(x => x.EveningMilk).GreaterThanOrEqualTo(0);
        RuleFor(x => x.FatPercent).InclusiveBetween(0, 100).When(x => x.FatPercent.HasValue);
        RuleFor(x => x.RatePerLitre).GreaterThanOrEqualTo(0).When(x => x.RatePerLitre.HasValue);
    }
}

public sealed class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    public CreateSaleRequestValidator()
    {
        RuleFor(x => x.SaleDate).NotEqual(default(DateTime)).WithMessage("SaleDate is required.");
        RuleFor(x => x.BuyerName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.QuantityLitres).GreaterThanOrEqualTo(0);
        RuleFor(x => x.RatePerLitre).GreaterThanOrEqualTo(0);
        RuleFor(x => x.PaymentStatus).Must(s => PaymentStatuses.All.Contains(s))
            .WithMessage($"PaymentStatus must be one of: {string.Join(", ", PaymentStatuses.All)}.");
    }
}

public sealed class UpdatePaymentStatusRequestValidator : AbstractValidator<UpdatePaymentStatusRequest>
{
    public UpdatePaymentStatusRequestValidator()
        => RuleFor(x => x.PaymentStatus).Must(s => PaymentStatuses.All.Contains(s))
            .WithMessage($"PaymentStatus must be one of: {string.Join(", ", PaymentStatuses.All)}.");
}

public sealed class CreateExpenseRequestValidator : AbstractValidator<CreateExpenseRequest>
{
    public CreateExpenseRequestValidator()
    {
        RuleFor(x => x.ExpenseDate).NotEqual(default(DateTime)).WithMessage("ExpenseDate is required.");
        RuleFor(x => x.Category).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
    }
}

public sealed class UpdateExpenseRequestValidator : AbstractValidator<UpdateExpenseRequest>
{
    public UpdateExpenseRequestValidator()
    {
        RuleFor(x => x.ExpenseDate).NotEqual(default(DateTime)).WithMessage("ExpenseDate is required.");
        RuleFor(x => x.Category).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateIncomeRequestValidator : AbstractValidator<CreateIncomeRequest>
{
    public CreateIncomeRequestValidator()
    {
        RuleFor(x => x.IncomeDate).NotEqual(default(DateTime)).WithMessage("IncomeDate is required.");
        RuleFor(x => x.Category).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
    }
}

public sealed class CreateHealthRecordRequestValidator : AbstractValidator<CreateHealthRecordRequest>
{
    public CreateHealthRecordRequestValidator()
    {
        RuleFor(x => x.RecordDate).NotEqual(default(DateTime)).WithMessage("RecordDate is required.");
        RuleFor(x => x.RecordType).NotEmpty().Must(t => HealthRecordTypes.All.Contains(t))
            .WithMessage($"RecordType must be one of: {string.Join(", ", HealthRecordTypes.All)}.");
        RuleFor(x => x.Cost).GreaterThanOrEqualTo(0).When(x => x.Cost.HasValue);
    }
}

public sealed class CreateBreedingRequestValidator : AbstractValidator<CreateBreedingRequest>
{
    public CreateBreedingRequestValidator()
    {
        RuleFor(x => x.InseminationDate).NotEqual(default(DateTime)).WithMessage("InseminationDate is required.");
        RuleFor(x => x.PregnancyStatus).Must(s => PregnancyStatuses.All.Contains(s))
            .WithMessage($"PregnancyStatus must be one of: {string.Join(", ", PregnancyStatuses.All)}.");
    }
}

public sealed class UpdateCalvingRequestValidator : AbstractValidator<UpdateCalvingRequest>
{
    public UpdateCalvingRequestValidator()
    {
        RuleFor(x => x.CalvingDate).NotEqual(default(DateTime)).WithMessage("CalvingDate is required.");
        RuleFor(x => x.PregnancyStatus).Must(s => PregnancyStatuses.All.Contains(s))
            .WithMessage($"PregnancyStatus must be one of: {string.Join(", ", PregnancyStatuses.All)}.");
    }
}

public sealed class CreateFeedItemRequestValidator : AbstractValidator<CreateFeedItemRequest>
{
    public CreateFeedItemRequestValidator()
    {
        RuleFor(x => x.ItemName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.QuantityInStock).GreaterThanOrEqualTo(0);
        RuleFor(x => x.MinimumLevel).GreaterThanOrEqualTo(0);
    }
}

public sealed class UpdateStockRequestValidator : AbstractValidator<UpdateStockRequest>
{
    public UpdateStockRequestValidator()
        => RuleFor(x => x.QuantityInStock).GreaterThanOrEqualTo(0);
}
