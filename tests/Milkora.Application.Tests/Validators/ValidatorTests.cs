using FluentAssertions;
using Milkora.Application.DTOs;
using Milkora.Application.Validators;
using Xunit;

namespace Milkora.Application.Tests.Validators;

public class ValidatorTests
{
    private static readonly CreateAnimalRequestValidator Animal = new();
    private static readonly CreateMilkLogRequestValidator MilkLog = new();
    private static readonly CreateSaleRequestValidator Sale = new();
    private static readonly CreateFeedItemRequestValidator Feed = new();

    [Fact]
    public void Animal_valid_request_passes()
    {
        var result = Animal.Validate(new CreateAnimalRequest { TagNumber = "COW-1", Status = "Milking", DailyMilkYield = 10 });
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Animal_empty_tag_fails()
    {
        var result = Animal.Validate(new CreateAnimalRequest { TagNumber = "", Status = "Milking" });
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateAnimalRequest.TagNumber));
    }

    [Fact]
    public void Animal_invalid_status_fails()
    {
        var result = Animal.Validate(new CreateAnimalRequest { TagNumber = "COW-1", Status = "Flying" });
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateAnimalRequest.Status));
    }

    [Fact]
    public void Animal_negative_price_fails()
    {
        var result = Animal.Validate(new CreateAnimalRequest { TagNumber = "COW-1", Status = "Dry", PurchasePrice = -5 });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void MilkLog_fat_percent_over_100_fails()
    {
        var result = MilkLog.Validate(new CreateMilkLogRequest { LogDate = DateTime.Today, FatPercent = 120 });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void MilkLog_negative_milk_fails()
    {
        var result = MilkLog.Validate(new CreateMilkLogRequest { LogDate = DateTime.Today, MorningMilk = -1 });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void MilkLog_valid_passes()
    {
        var result = MilkLog.Validate(new CreateMilkLogRequest { LogDate = DateTime.Today, MorningMilk = 7, EveningMilk = 6, FatPercent = 4.2m, RatePerLitre = 55 });
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Sale_empty_buyer_fails()
    {
        var result = Sale.Validate(new CreateSaleRequest { SaleDate = DateTime.Today, BuyerName = "", PaymentStatus = "Pending" });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Sale_invalid_payment_status_fails()
    {
        var result = Sale.Validate(new CreateSaleRequest { SaleDate = DateTime.Today, BuyerName = "X", PaymentStatus = "Later" });
        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Feed_empty_name_fails()
    {
        var result = Feed.Validate(new CreateFeedItemRequest { ItemName = "", QuantityInStock = 10, MinimumLevel = 5 });
        result.IsValid.Should().BeFalse();
    }
}
