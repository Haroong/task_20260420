using FluentAssertions;
using task_20260420.Services;

namespace task_20260420.Tests.Unit.Services;

public class JsonEmployeeParserTests
{
    private readonly JsonEmployeeParser _parser = new();

    [Fact]
    public void CanParse_WithJsonFileName_ReturnsTrue()
    {
        _parser.CanParse("any content", "employees.json").Should().BeTrue();
    }

    [Fact]
    public void CanParse_WithCsvFileName_ReturnsFalse()
    {
        _parser.CanParse("any content", "employees.csv").Should().BeFalse();
    }

    [Fact]
    public void CanParse_WithArrayContent_ReturnsTrue()
    {
        _parser.CanParse("[{\"name\":\"test\"}]", null).Should().BeTrue();
    }

    [Fact]
    public void CanParse_WithObjectContent_ReturnsTrue()
    {
        _parser.CanParse("{\"name\":\"test\"}", null).Should().BeTrue();
    }

    [Fact]
    public void CanParse_WithPlainText_ReturnsFalse()
    {
        _parser.CanParse("김철수, test@test.com", null).Should().BeFalse();
    }

    [Fact]
    public void Parse_ValidJsonArray_ReturnsEmployees()
    {
        var json = """
                   [
                     {"name":"김클로", "email":"clo@clovf.com", "tel":"010-1111-2424", "joined":"2012-01-05"},
                     {"name":"박마블", "email":"md@clovf.com", "tel":"010-3535-7979", "joined":"2013-07-01"},
                     {"name":"홍커넥", "email":"connect@clovf.com", "tel":"010-8531-7942", "joined":"2019-12-05"}
                   ]
                   """;

        var result = _parser.Parse(json);

        result.Should().HaveCount(3);
        result[0].Name.Should().Be("김클로");
        result[0].Email.Should().Be("clo@clovf.com");
        result[0].Tel.Should().Be("010-1111-2424");
        result[0].Joined.Should().Be(new DateTime(2012, 1, 5));
    }

    [Fact]
    public void Parse_SingleObject_ReturnsOneEmployee()
    {
        var json = """{"name":"김클로", "email":"clo@clovf.com", "tel":"010-1111-2424", "joined":"2012-01-05"}""";

        var result = _parser.Parse(json);

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("김클로");
    }

    [Fact]
    public void Parse_DotDateFormat_ParsesCorrectly()
    {
        var json = """[{"name":"김철수", "email":"test@test.com", "tel":"01012345678", "joined":"2018.03.07"}]""";

        var result = _parser.Parse(json);

        result[0].Joined.Should().Be(new DateTime(2018, 3, 7));
    }

    [Fact]
    public void Parse_EmptyString_ThrowsFormatException()
    {
        var act = () => _parser.Parse("");

        act.Should().Throw<FormatException>();
    }

    [Fact]
    public void Parse_InvalidJson_ThrowsFormatException()
    {
        var act = () => _parser.Parse("{invalid json}");

        act.Should().Throw<FormatException>().WithMessage("*JSON*");
    }

    [Fact]
    public void Parse_MissingName_ThrowsFormatException()
    {
        var json = """[{"email":"test@test.com", "tel":"01012345678", "joined":"2020-01-01"}]""";

        var act = () => _parser.Parse(json);

        act.Should().Throw<FormatException>().WithMessage("*이름*");
    }

    [Fact]
    public void Parse_EmptyArray_ThrowsFormatException()
    {
        var act = () => _parser.Parse("[]");

        act.Should().Throw<FormatException>().WithMessage("*데이터*없*");
    }
}
