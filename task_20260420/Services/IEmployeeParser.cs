using task_20260420.Domain;

namespace task_20260420.Services;

public interface IEmployeeParser
{
    bool CanParse(string content, string? fileName);
    List<Employee> Parse(string content);
}
