using FluentAssertions;
using Milkora.Application.DTOs;
using Milkora.Application.Services;
using Milkora.Domain.Entities;
using Milkora.Domain.Exceptions;
using Milkora.Domain.Interfaces;
using NSubstitute;
using Xunit;

namespace Milkora.Application.Tests.Services;

public class SaleServiceTests
{
    private readonly ISaleRepository _repo = Substitute.For<ISaleRepository>();
    private readonly SaleService _sut;
    public SaleServiceTests() => _sut = new SaleService(_repo);

    [Fact]
    public async Task Create_returns_new_id()
    {
        var newId = Guid.NewGuid();
        _repo.InsertAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(newId);

        var id = await _sut.CreateAsync(new CreateSaleRequest { BuyerName = "Co-op", PaymentStatus = "Pending" });

        id.Should().Be(newId);
    }

    [Fact]
    public async Task UpdatePaymentStatus_throws_NotFound_when_no_rows()
    {
        _repo.UpdatePaymentStatusAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(0);

        var act = () => _sut.UpdatePaymentStatusAsync(Guid.NewGuid(), new UpdatePaymentStatusRequest { PaymentStatus = "Paid" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task UpdatePaymentStatus_succeeds_when_row_affected()
    {
        _repo.UpdatePaymentStatusAsync(Arg.Any<Guid>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>())
            .Returns(1);

        var act = () => _sut.UpdatePaymentStatusAsync(Guid.NewGuid(), new UpdatePaymentStatusRequest { PaymentStatus = "Paid", PaymentMode = "Cash" });

        await act.Should().NotThrowAsync();
    }
}

public class ReportServiceTests
{
    private readonly IReportRepository _repo = Substitute.For<IReportRepository>();
    private readonly ReportService _sut;
    public ReportServiceTests() => _sut = new ReportService(_repo);

    [Fact]
    public async Task GetDashboard_throws_NotFound_when_repo_returns_null()
    {
        _repo.GetDashboardSummaryAsync(Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
            .Returns((DashboardSummary?)null);

        var act = () => _sut.GetDashboardAsync(DateTime.Today);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task GetDashboard_maps_summary_when_present()
    {
        _repo.GetDashboardSummaryAsync(Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
            .Returns(new DashboardSummary { TotalMilkLitres = 21.3m, TotalIncome = 1600, TotalExpense = 1350, ProfitOrLoss = 250, ActiveAnimals = 3 });

        var dto = await _sut.GetDashboardAsync(DateTime.Today);

        dto.TotalMilkLitres.Should().Be(21.3m);
        dto.ProfitOrLoss.Should().Be(250);
        dto.ActiveAnimals.Should().Be(3);
    }
}
