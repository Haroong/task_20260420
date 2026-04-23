using MediatR;

namespace task_20260420.Features.Employees.Commands.AddEmployees;

public record AddEmployeesCommand(string Content, string? FileName) : IRequest<AddEmployeesResult>;

/// <summary>직원 등록 결과</summary>
/// <param name="AddedCount">추가된 직원 수 <example>3</example></param>
public record AddEmployeesResult(int AddedCount);
