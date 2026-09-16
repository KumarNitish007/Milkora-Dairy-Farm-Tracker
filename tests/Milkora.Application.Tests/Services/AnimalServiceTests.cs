using FluentAssertions;
using Milkora.Application.DTOs;
using Milkora.Application.Services;
using Milkora.Domain.Entities;
using Milkora.Domain.Exceptions;
using Milkora.Domain.Interfaces;
using NSubstitute;
using Xunit;

namespace Milkora.Application.Tests.Services;

public class AnimalServiceTests
{
    private readonly IAnimalRepository _repo = Substitute.For<IAnimalRepository>();
    private readonly AnimalService _sut;

    public AnimalServiceTests() => _sut = new AnimalService(_repo);

    private static Animal SampleAnimal(Guid id) => new()
    {
        AnimalId = id, TagNumber = "COW-001", Name = "Ganga", Status = "Milking", DailyMilkYield = 12.5m,
    };

    [Fact]
    public async Task GetAll_maps_entities_to_dtos()
    {
        _repo.GetAllAsync(null, null, Arg.Any<CancellationToken>())
            .Returns(new List<Animal> { SampleAnimal(Guid.NewGuid()), SampleAnimal(Guid.NewGuid()) });

        var result = await _sut.GetAllAsync(null, null);

        result.Should().HaveCount(2);
        result[0].Should().BeOfType<AnimalDto>();
        result[0].TagNumber.Should().Be("COW-001");
    }

    [Fact]
    public async Task GetById_returns_dto_when_found()
    {
        var id = Guid.NewGuid();
        _repo.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns(SampleAnimal(id));

        var dto = await _sut.GetByIdAsync(id);

        dto.AnimalId.Should().Be(id);
        dto.Name.Should().Be("Ganga");
    }

    [Fact]
    public async Task GetById_throws_NotFound_when_missing()
    {
        _repo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns((Animal?)null);

        var act = () => _sut.GetByIdAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Create_passes_mapped_entity_and_returns_new_id()
    {
        var newId = Guid.NewGuid();
        _repo.InsertAsync(Arg.Any<Animal>(), Arg.Any<CancellationToken>()).Returns(newId);
        var request = new CreateAnimalRequest { TagNumber = "COW-9", Status = "Milking" };

        var id = await _sut.CreateAsync(request);

        id.Should().Be(newId);
        await _repo.Received(1).InsertAsync(Arg.Is<Animal>(a => a.TagNumber == "COW-9"), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_throws_NotFound_when_no_rows_affected()
    {
        _repo.UpdateAsync(Arg.Any<Animal>(), Arg.Any<CancellationToken>()).Returns(0);

        var act = () => _sut.UpdateAsync(Guid.NewGuid(), new UpdateAnimalRequest { TagNumber = "X", Status = "Dry" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Delete_throws_NotFound_when_no_rows_affected()
    {
        _repo.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(0);

        var act = () => _sut.DeleteAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Delete_succeeds_when_row_affected()
    {
        _repo.DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>()).Returns(1);

        var act = () => _sut.DeleteAsync(Guid.NewGuid());

        await act.Should().NotThrowAsync();
    }
}
