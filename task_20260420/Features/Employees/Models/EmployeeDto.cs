using task_20260420.Domain;

namespace task_20260420.Features.Employees.Models;

/// <summary>직원 정보</summary>
/// <param name="Name">직원 이름 <example>김민수</example></param>
/// <param name="Email">이메일 주소 <example>minsu.kim@company.com</example></param>
/// <param name="Tel">전화번호 <example>010-1234-5678</example></param>
/// <param name="Joined">입사일 (yyyy-MM-dd) <example>2023-03-15</example></param>
public record EmployeeDto(
    string Name,
    string Email,
    string Tel,
    string Joined)
{
    public static EmployeeDto FromEntity(Employee e) =>
        new(e.Name, e.Email, e.Tel, e.Joined.ToString("yyyy-MM-dd"));
}
