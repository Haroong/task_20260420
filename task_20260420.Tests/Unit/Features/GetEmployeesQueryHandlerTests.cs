using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using task_20260420.Domain;
using task_20260420.Features.Employees.Queries.GetEmployees;
using task_20260420.Infrastructure;

namespace task_20260420.Tests.Unit.Features;

public class GetEmployeesQueryHandlerTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly GetEmployeesQueryHandler _handler;

    public GetEmployeesQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new AppDbContext(options);
        _handler = new GetEmployeesQueryHandler(_db);

        SeedData();
    }

    private void SeedData()
    {
        _db.Employees.AddRange(
            new Employee { Name = "김철수", Email = "a@test.com", Tel = "010-1111-1111", Joined = new DateTime(2020, 1, 1) },
            new Employee { Name = "박영희", Email = "b@test.com", Tel = "010-2222-2222", Joined = new DateTime(2021, 5, 15) },
            new Employee { Name = "홍길동", Email = "c@test.com", Tel = "010-3333-3333", Joined = new DateTime(2019, 3, 10) }
        );
        _db.SaveChanges();
    }

    [Fact]
    public async Task Handle_ReturnsPagedResult()
    {
        var query = new GetEmployeesQuery(1, 10);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.TotalCount.Should().Be(3);
        result.Items.Should().HaveCount(3);
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(1);
    }

    [Fact]
    public async Task Handle_Pagination_ReturnsCorrectPage()
    {
        var query = new GetEmployeesQuery(1, 2);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task Handle_SecondPage_ReturnsRemainingItems()
    {
        var query = new GetEmployeesQuery(2, 2);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task Handle_EmptyDatabase_ReturnsEmptyResult()
    {
        _db.Employees.RemoveRange(_db.Employees);
        await _db.SaveChangesAsync();

        var query = new GetEmployeesQuery(1, 10);
        var result = await _handler.Handle(query, CancellationToken.None);

        result.TotalCount.Should().Be(0);
        result.Items.Should().BeEmpty();
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
