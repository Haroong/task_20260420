using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using task_20260420.Features.Employees.Commands.AddEmployees;
using task_20260420.Infrastructure;
using task_20260420.Services;

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

        var parsers = new IEmployeeParser[] { new CsvEmployeeParser(), new JsonEmployeeParser() };
        var parserFactory = new EmployeeParserFactory(parsers);
        _handler = new AddEmployeesCommandHandler(_db, parserFactory);
    }

    [Fact]
    public async Task Handle_ValidCsv_AddsToDatabase()
    {
        var csv = "김철수, charles@test.com, 01012345678, 2020.01.01\n박영희, younghee@test.com, 01087654321, 2021.05.15";
        var command = new AddEmployeesCommand(csv, "employees.csv");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.AddedCount.Should().Be(2);
        _db.Employees.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_ValidJson_AddsToDatabase()
    {
        var json = """[{"name":"홍길동","email":"gildong@test.com","tel":"01011112222","joined":"2019-03-10"}]""";
        var command = new AddEmployeesCommand(json, "employees.json");

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
