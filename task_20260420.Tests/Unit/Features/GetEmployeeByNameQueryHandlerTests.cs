using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using task_20260420.Domain;
using task_20260420.Features.Employees.Queries.GetEmployeeByName;
using task_20260420.Infrastructure;

namespace task_20260420.Tests.Unit.Features;

public class GetEmployeeByNameQueryHandlerTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly GetEmployeeByNameQueryHandler _handler;

    public GetEmployeeByNameQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new AppDbContext(options);
        _handler = new GetEmployeeByNameQueryHandler(_db);

        _db.Employees.Add(new Employee
        {
            Name = "김철수", Email = "charles@test.com", Tel = "01012345678", Joined = new DateTime(2020, 1, 1)
        });
        _db.SaveChanges();
    }

    [Fact]
    public async Task Handle_ExistingName_ReturnsEmployee()
    {
        var query = new GetEmployeeByNameQuery("김철수");

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Should().NotBeNull();
        result.Name.Should().Be("김철수");
        result.Email.Should().Be("charles@test.com");
        result.Tel.Should().Be("01012345678");
        result.Joined.Should().Be("2020-01-01");
    }

    [Fact]
    public async Task Handle_NonExistingName_ThrowsKeyNotFoundException()
    {
        var query = new GetEmployeeByNameQuery("존재하지않는이름");

        var act = () => _handler.Handle(query, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage("*존재하지않는이름*");
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
