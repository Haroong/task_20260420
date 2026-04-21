using MediatR;
using task_20260420.Features.Employees.Models;

namespace task_20260420.Features.Employees.Queries.GetEmployeeByName;

public record GetEmployeeByNameQuery(string Name) : IRequest<EmployeeDto>;
