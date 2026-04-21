using MediatR;
using task_20260420.Domain;

namespace task_20260420.Features.Employees.Commands.AddEmployees;

public record AddEmployeesCommand(List<Employee> Employees) : IRequest<AddEmployeesResult>;

public record AddEmployeesResult(int AddedCount);
