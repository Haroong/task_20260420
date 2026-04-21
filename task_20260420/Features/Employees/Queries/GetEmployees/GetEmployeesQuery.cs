using MediatR;
using task_20260420.Features.Employees.Models;

namespace task_20260420.Features.Employees.Queries.GetEmployees;

public record GetEmployeesQuery(int Page, int PageSize) : IRequest<PagedResult<EmployeeDto>>;
