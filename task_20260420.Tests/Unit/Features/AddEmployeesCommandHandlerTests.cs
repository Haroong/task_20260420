using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using task_20260420.Domain;
using task_20260420.Features.Employees.Commands.AddEmployees;
using task_20260420.Infrastructure;

namespace task_20260420.Tests.Unit.Features;

public class AddEmployeesCommandHandlerTests : IDisposable
{
    private readonly AppDbContext _db;
    private readonly AddEmployeesCommandHandler _handler;

    public AddEmployeesCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _db = new AppDbContext(options);
        _handler = new AddEmployeesCommandHandler(_db);
    }

    [Fact]
    public async Task Handle_ValidEmployees_AddsToDatabase()
    {
        var employees = new List<Employee>
        {
            new() { Name = "김철수", Email = "charles@test.com", Tel = "01012345678", Joined = new DateTime(2020, 1, 1) },
            new() { Name = "박영희", Email = "younghee@test.com", Tel = "01087654321", Joined = new DateTime(2021, 5, 15) }
        };
        var command = new AddEmployeesCommand(employees);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.AddedCount.Should().Be(2);
        _db.Employees.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_SingleEmployee_AddsToDatabase()
    {
        var employees = new List<Employee>
        {
            new() { Name = "홍길동", Email = "gildong@test.com", Tel = "01011112222", Joined = new DateTime(2019, 3, 10) }
        };
        var command = new AddEmployeesCommand(employees);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.AddedCount.Should().Be(1);
        var saved = await _db.Employees.FirstAsync();
        saved.Name.Should().Be("홍길동");
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
