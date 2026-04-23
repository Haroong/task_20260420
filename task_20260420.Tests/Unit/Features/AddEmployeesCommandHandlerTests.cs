using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using task_20260420.Domain;
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

    [Fact]
    public async Task Handle_DuplicateEmailInDb_UpdatesExisting()
    {
        // Arrange: DB에 기존 직원 등록
        _db.Employees.Add(new Employee
        {
            Name = "김철수", Email = "charles@test.com", Tel = "01000000000", Joined = new DateTime(2020, 1, 1)
        });
        await _db.SaveChangesAsync();

        // Act: 같은 이메일로 새 데이터 업로드
        var csv = "김철수_수정, charles@test.com, 01099999999, 2024.06.01";
        var command = new AddEmployeesCommand(csv, "update.csv");
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AddedCount.Should().Be(0);
        result.UpdatedCount.Should().Be(1);
        _db.Employees.Should().HaveCount(1);
        var employee = await _db.Employees.FirstAsync();
        employee.Name.Should().Be("김철수_수정");
        employee.Tel.Should().Be("01099999999");
    }

    [Fact]
    public async Task Handle_MixOfNewAndExisting_ReturnsCorrectCounts()
    {
        // Arrange
        _db.Employees.Add(new Employee
        {
            Name = "기존직원", Email = "existing@test.com", Tel = "01011111111", Joined = new DateTime(2020, 1, 1)
        });
        await _db.SaveChangesAsync();

        // Act: 기존 1명(이메일 동일) + 신규 1명
        var csv = "기존직원_수정, existing@test.com, 01022222222, 2024.01.01\n신규직원, new@test.com, 01033333333, 2024.02.01";
        var command = new AddEmployeesCommand(csv, "mix.csv");
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.AddedCount.Should().Be(1);
        result.UpdatedCount.Should().Be(1);
        _db.Employees.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_DuplicateEmailWithinBatch_LastOneWins()
    {
        // Act: 같은 이메일이 배치 내에 2번 등장
        var csv = "첫번째, dup@test.com, 01011111111, 2024.01.01\n두번째, dup@test.com, 01022222222, 2024.06.01";
        var command = new AddEmployeesCommand(csv, "batch-dup.csv");
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert: 마지막 행이 반영
        result.AddedCount.Should().Be(1);
        _db.Employees.Should().HaveCount(1);
        var employee = await _db.Employees.FirstAsync();
        employee.Name.Should().Be("두번째");
    }

    [Fact]
    public async Task Handle_EmailCaseInsensitive_TreatsAsDuplicate()
    {
        // Arrange
        _db.Employees.Add(new Employee
        {
            Name = "원본", Email = "test@example.com", Tel = "01011111111", Joined = new DateTime(2020, 1, 1)
        });
        await _db.SaveChangesAsync();

        // Act: 대소문자만 다른 이메일
        var csv = "수정됨, TEST@EXAMPLE.COM, 01099999999, 2024.01.01";
        var command = new AddEmployeesCommand(csv, "case.csv");
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.UpdatedCount.Should().Be(1);
        result.AddedCount.Should().Be(0);
        _db.Employees.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_AllNew_ReturnsZeroUpdated()
    {
        var csv = "직원A, a@test.com, 01011111111, 2024.01.01\n직원B, b@test.com, 01022222222, 2024.02.01";
        var command = new AddEmployeesCommand(csv, "new.csv");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.AddedCount.Should().Be(2);
        result.UpdatedCount.Should().Be(0);
        _db.Employees.Should().HaveCount(2);
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}
