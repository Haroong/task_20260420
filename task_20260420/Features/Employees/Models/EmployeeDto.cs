using task_20260420.Domain;

namespace task_20260420.Features.Employees.Models;

public record EmployeeDto(
    string Name,
    string Email,
    string Tel,
    string Joined)
{
    public static EmployeeDto FromEntity(Employee e) =>
        new(e.Name, e.Email, e.Tel, e.Joined.ToString("yyyy-MM-dd"));
}
