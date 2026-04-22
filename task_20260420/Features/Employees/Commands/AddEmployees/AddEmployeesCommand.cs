using MediatR;

namespace task_20260420.Features.Employees.Commands.AddEmployees;

public record AddEmployeesCommand(string Content, string? FileName) : IRequest<AddEmployeesResult>;

public record AddEmployeesResult(int AddedCount);
