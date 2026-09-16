using Microsoft.Data.SqlClient;
using Milkora.Domain.Entities;
using Milkora.Domain.Interfaces;
using Milkora.Infrastructure.Persistence;

namespace Milkora.Infrastructure.Repositories;

public sealed class AnimalRepository : RepositoryBase, IAnimalRepository
{
    public AnimalRepository(IDbConnectionFactory factory) : base(factory) { }

    private static Animal Map(SqlDataReader r) => new()
    {
        AnimalId       = r.GetGuid("AnimalId"),
        TagNumber      = r.GetString("TagNumber"),
        Name           = r.GetNullableString("Name"),
        Type           = r.GetNullableString("Type"),
        Breed          = r.GetNullableString("Breed"),
        Gender         = r.GetNullableString("Gender"),
        DateOfBirth    = r.GetNullableDateTime("DateOfBirth"),
        PurchaseDate   = r.GetNullableDateTime("PurchaseDate"),
        PurchasePrice  = r.GetNullableDecimal("PurchasePrice"),
        Status         = r.GetString("Status"),
        DailyMilkYield = r.GetNullableDecimal("DailyMilkYield"),
        PhotoPath      = r.GetNullableString("PhotoPath"),
        Notes          = r.GetNullableString("Notes"),
        CreatedAt      = r.GetDateTime("CreatedAt"),
        UpdatedAt      = r.GetDateTime("UpdatedAt"),
    };

    public Task<IReadOnlyList<Animal>> GetAllAsync(string? status, string? search, CancellationToken ct = default)
        => QueryListAsync("dbo.sp_GetAllAnimals", cmd =>
        {
            AddParam(cmd, "@Status", status);
            AddParam(cmd, "@Search", search);
        }, Map, ct);

    public Task<Animal?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => QuerySingleAsync("dbo.sp_GetAnimalById", cmd => AddParam(cmd, "@AnimalId", id), Map, ct);

    public Task<Guid> InsertAsync(Animal a, CancellationToken ct = default)
        => ExecuteInsertAsync("dbo.sp_InsertAnimal", cmd =>
        {
            AddParam(cmd, "@AnimalId", a.AnimalId == Guid.Empty ? null : a.AnimalId);
            AddParam(cmd, "@TagNumber", a.TagNumber);
            AddParam(cmd, "@Name", a.Name);
            AddParam(cmd, "@Type", a.Type);
            AddParam(cmd, "@Breed", a.Breed);
            AddParam(cmd, "@Gender", a.Gender);
            AddParam(cmd, "@DateOfBirth", a.DateOfBirth);
            AddParam(cmd, "@PurchaseDate", a.PurchaseDate);
            AddParam(cmd, "@PurchasePrice", a.PurchasePrice);
            AddParam(cmd, "@Status", a.Status);
            AddParam(cmd, "@DailyMilkYield", a.DailyMilkYield);
            AddParam(cmd, "@PhotoPath", a.PhotoPath);
            AddParam(cmd, "@Notes", a.Notes);
        }, ct);

    public Task<int> UpdateAsync(Animal a, CancellationToken ct = default)
        => ExecuteRowCountAsync("dbo.sp_UpdateAnimal", cmd =>
        {
            AddParam(cmd, "@AnimalId", a.AnimalId);
            AddParam(cmd, "@TagNumber", a.TagNumber);
            AddParam(cmd, "@Name", a.Name);
            AddParam(cmd, "@Type", a.Type);
            AddParam(cmd, "@Breed", a.Breed);
            AddParam(cmd, "@Gender", a.Gender);
            AddParam(cmd, "@DateOfBirth", a.DateOfBirth);
            AddParam(cmd, "@PurchaseDate", a.PurchaseDate);
            AddParam(cmd, "@PurchasePrice", a.PurchasePrice);
            AddParam(cmd, "@Status", a.Status);
            AddParam(cmd, "@DailyMilkYield", a.DailyMilkYield);
            AddParam(cmd, "@PhotoPath", a.PhotoPath);
            AddParam(cmd, "@Notes", a.Notes);
        }, ct);

    public Task<int> DeleteAsync(Guid id, CancellationToken ct = default)
        => ExecuteRowCountAsync("dbo.sp_DeleteAnimal", cmd => AddParam(cmd, "@AnimalId", id), ct);
}
