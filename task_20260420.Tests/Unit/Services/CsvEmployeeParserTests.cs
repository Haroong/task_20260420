using FluentAssertions;
using task_20260420.Services;

namespace task_20260420.Tests.Unit.Services;

public class CsvEmployeeParserTests
{
    private readonly CsvEmployeeParser _parser = new();

    [Fact]
    public void CanParse_WithCsvFileName_ReturnsTrue()
    {
        _parser.CanParse("any content", "employees.csv").Should().BeTrue();
    }

    [Fact]
    public void CanParse_WithJsonFileName_ReturnsFalse()
    {
        _parser.CanParse("any content", "employees.json").Should().BeFalse();
    }

    [Fact]
    public void CanParse_WithPlainText_ReturnsTrue()
    {
        _parser.CanParse("김철수, test@test.com, 01012345678, 2020.01.01", null).Should().BeTrue();
    }

    [Fact]
    public void CanParse_WithJsonContent_ReturnsFalse()
    {
        _parser.CanParse("[{\"name\":\"test\"}]", null).Should().BeFalse();
    }

    [Fact]
    public void Parse_ValidCsv_ReturnsEmployees()
    {
        var csv = """
                  김철수, charles@clovf.com, 01075312468, 2018.03.07
                  박영희, matilda@clovf.com, 01087654321, 2021.04.28
                  홍길동, kildong.hong@clovf.com, 01012345678, 2015.08.15
                  """;

        var result = _parser.Parse(csv);

        result.Should().HaveCount(3);
        result[0].Name.Should().Be("김철수");
        result[0].Email.Should().Be("charles@clovf.com");
        result[0].Tel.Should().Be("01075312468");
        result[0].Joined.Should().Be(new DateTime(2018, 3, 7));
    }

    [Fact]
    public void Parse_SingleRow_ReturnsOneEmployee()
    {
        var csv = "홍길동, test@test.com, 01012345678, 2020.01.01";

        var result = _parser.Parse(csv);

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("홍길동");
    }

    [Fact]
    public void Parse_DashDateFormat_ParsesCorrectly()
    {
        var csv = "김철수, test@test.com, 01012345678, 2020-05-15";

        var result = _parser.Parse(csv);

        result[0].Joined.Should().Be(new DateTime(2020, 5, 15));
    }

    [Fact]
    public void Parse_EmptyString_ThrowsFormatException()
    {
        var act = () => _parser.Parse("");

        act.Should().Throw<FormatException>().WithMessage("*비어*");
    }

    [Fact]
    public void Parse_WhitespaceOnly_ThrowsFormatException()
    {
        var act = () => _parser.Parse("   ");

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_InvalidDate_ThrowsFormatException()
    {
        var csv = "김철수, test@test.com, 01012345678, invalid-date";

        var act = () => _parser.Parse(csv);

        act.Should().Throw<FormatException>().WithMessage("*날짜*");
    }

    [Fact]
    public void Parse_MissingColumns_ThrowsFormatException()
    {
        var csv = "김철수, test@test.com";

        var act = () => _parser.Parse(csv);

        act.Should().Throw<FormatException>();
    }
}
