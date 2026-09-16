using FluentAssertions;
using Milkora.Application.Common;
using Milkora.Application.DTOs;
using Milkora.Domain.Entities;
using Xunit;

namespace Milkora.Application.Tests;

public class MappingTests
{
    [Fact]
    public void Animal_ToDto_copies_all_fields()
    {
        var id = Guid.NewGuid();
        var entity = new Animal
        {
            AnimalId = id, TagNumber = "BUF-001", Name = "Lakshmi", Type = "Buffalo", Breed = "Murrah",
            Gender = "Female", Status = "Milking", DailyMilkYield = 8.2m, PurchasePrice = 92000,
        };

        var dto = entity.ToDto();

        dto.AnimalId.Should().Be(id);
        dto.TagNumber.Should().Be("BUF-001");
        dto.Breed.Should().Be("Murrah");
        dto.DailyMilkYield.Should().Be(8.2m);
    }

    [Fact]
    public void CreateRequest_ToEntity_uses_EmptyGuid_when_id_omitted()
    {
        var entity = new CreateAnimalRequest { TagNumber = "COW-1", Status = "Milking" }.ToEntity();
        entity.AnimalId.Should().Be(Guid.Empty);
    }

    [Fact]
    public void CreateRequest_ToEntity_honours_client_supplied_guid()
    {
        var clientId = Guid.NewGuid();
        var entity = new CreateAnimalRequest { AnimalId = clientId, TagNumber = "COW-1", Status = "Milking" }.ToEntity();
        entity.AnimalId.Should().Be(clientId);
    }

    [Fact]
    public void UpdateRequest_ToEntity_sets_the_given_id()
    {
        var id = Guid.NewGuid();
        var entity = new UpdateAnimalRequest { TagNumber = "COW-1", Status = "Dry" }.ToEntity(id);
        entity.AnimalId.Should().Be(id);
        entity.Status.Should().Be("Dry");
    }
}

public class ApiResponseTests
{
    [Fact]
    public void Ok_with_data_sets_success_and_payload()
    {
        var r = ApiResponse<int>.Ok(42, "done");
        r.Success.Should().BeTrue();
        r.Data.Should().Be(42);
        r.Message.Should().Be("done");
        r.Errors.Should().BeNull();
    }

    [Fact]
    public void Fail_sets_success_false_and_errors()
    {
        var errors = new Dictionary<string, string[]> { ["TagNumber"] = new[] { "required" } };
        var r = ApiResponse.Fail("invalid", errors);
        r.Success.Should().BeFalse();
        r.Message.Should().Be("invalid");
        r.Errors!.Should().ContainKey("TagNumber");
    }
}
