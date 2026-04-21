using FluentAssertions;
using task_20260420.Services;

namespace task_20260420.Tests.Unit.Services;

public class EmployeeParserFactoryTests
{
    private readonly EmployeeParserFactory _factory;

    public EmployeeParserFactoryTests()
    {
        var parsers = new IEmployeeParser[] { new CsvEmployeeParser(), new JsonEmployeeParser() };
        _factory = new EmployeeParserFactory(parsers);
    }

    [Fact]
    public void Parse_CsvFile_UsesCorrectParser()
    {
        var csv = "김철수, test@test.com, 01012345678, 2020.01.01";

        var result = _factory.Parse(csv, "data.csv");

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("김철수");
    }

    [Fact]
    public void Parse_JsonFile_UsesCorrectParser()
    {
        var json = """[{"name":"김철수", "email":"test@test.com", "tel":"01012345678", "joined":"2020-01-01"}]""";

        var result = _factory.Parse(json, "data.json");

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("김철수");
    }

    [Fact]
    public void Parse_AutoDetectCsv_WhenNoFileName()
    {
        var csv = "김철수, test@test.com, 01012345678, 2020.01.01";

        var result = _factory.Parse(csv);

        result.Should().HaveCount(1);
    }

    [Fact]
    public void Parse_AutoDetectJson_WhenNoFileName()
    {
        var json = """[{"name":"김철수", "email":"test@test.com", "tel":"01012345678", "joined":"2020-01-01"}]""";

        var result = _factory.Parse(json);

        result.Should().HaveCount(1);
    }
}
