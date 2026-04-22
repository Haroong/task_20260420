using task_20260420.Common.Exceptions;
using task_20260420.Domain;

namespace task_20260420.Services;

public class EmployeeParserFactory
{
    private readonly IEnumerable<IEmployeeParser> _parsers;

    public EmployeeParserFactory(IEnumerable<IEmployeeParser> parsers)
    {
        _parsers = parsers;
    }

    public List<Employee> Parse(string content, string? fileName = null)
    {
        var parser = _parsers.FirstOrDefault(p => p.CanParse(content, fileName));

        if (parser is null)
            throw new InvalidFormatException("지원하지 않는 데이터 형식입니다. CSV 또는 JSON 형식을 사용해 주세요.");

        return parser.Parse(content);
    }
}
