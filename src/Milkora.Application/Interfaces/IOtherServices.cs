using Milkora.Application.DTOs;

namespace Milkora.Application.Interfaces;

public interface IMilkLogService
{
    Task<IReadOnlyList<MilkLogDto>> GetByDateAsync(DateTime date, CancellationToken ct = default);
    Task<IReadOnlyList<MilkLogDto>> GetByDateRangeAsync(DateTime start, DateTime end, Guid? animalId, CancellationToken ct = default);
    Task<Guid> CreateAsync(CreateMilkLogRequest request, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpdateMilkLogRequest request, CancellationToken ct = default);
}

public interface ISaleService
{
    Task<IReadOnlyList<SaleDto>> GetByDateRangeAsync(DateTime start, DateTime end, string? paymentStatus, string? buyer, CancellationToken ct = default);
    Task<Guid> CreateAsync(CreateSaleRequest request, CancellationToken ct = default);
    Task UpdatePaymentStatusAsync(Guid id, UpdatePaymentStatusRequest request, CancellationToken ct = default);
}

public interface IExpenseService
{
    Task<IReadOnlyList<ExpenseDto>> GetByDateRangeAsync(DateTime start, DateTime end, string? category, CancellationToken ct = default);
    Task<Guid> CreateAsync(CreateExpenseRequest request, CancellationToken ct = default);
    Task UpdateAsync(Guid id, UpdateExpenseRequest request, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IIncomeService
{
    Task<IReadOnlyList<IncomeDto>> GetByDateRangeAsync(DateTime start, DateTime end, string? category, CancellationToken ct = default);
    Task<Guid> CreateAsync(CreateIncomeRequest request, CancellationToken ct = default);
}

public interface IHealthService
{
    Task<IReadOnlyList<HealthRecordDto>> GetByAnimalAsync(Guid? animalId, string? recordType, CancellationToken ct = default);
    Task<IReadOnlyList<DueReminderDto>> GetDueRemindersAsync(DateTime? asOf, int daysAhead, CancellationToken ct = default);
    Task<Guid> CreateAsync(CreateHealthRecordRequest request, CancellationToken ct = default);
}

public interface IBreedingService
{
    Task<IReadOnlyList<BreedingRecordDto>> GetByAnimalAsync(Guid? animalId, CancellationToken ct = default);
    Task<Guid> CreateAsync(CreateBreedingRequest request, CancellationToken ct = default);
    Task UpdateCalvingAsync(Guid id, UpdateCalvingRequest request, CancellationToken ct = default);
}

public interface IFeedService
{
    Task<IReadOnlyList<FeedItemDto>> GetAllAsync(string? search, CancellationToken ct = default);
    Task<IReadOnlyList<FeedItemDto>> GetLowStockAsync(CancellationToken ct = default);
    Task<Guid> CreateAsync(CreateFeedItemRequest request, CancellationToken ct = default);
    Task UpdateStockAsync(Guid id, UpdateStockRequest request, CancellationToken ct = default);
}

public interface IReportService
{
    Task<DashboardSummaryDto> GetDashboardAsync(DateTime? date, CancellationToken ct = default);
    Task<MonthlyReportDto> GetMonthlyAsync(int year, int month, CancellationToken ct = default);
    Task<ProfitLossReportDto> GetProfitLossAsync(DateTime start, DateTime end, CancellationToken ct = default);
    Task<IReadOnlyList<PerAnimalYieldDto>> GetPerAnimalYieldAsync(DateTime start, DateTime end, CancellationToken ct = default);
}
