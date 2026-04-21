using MediatR;
using Microsoft.EntityFrameworkCore;
using task_20260420.Features.Employees.Models;
using task_20260420.Infrastructure;

namespace task_20260420.Features.Employees.Queries.GetEmployees;

public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, PagedResult<EmployeeDto>>
{
    private readonly AppDbContext _db;

    public GetEmployeesQueryHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<EmployeeDto>> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
    {
        var totalCount = await _db.Employees.CountAsync(cancellationToken);

        var entities = await _db.Employees
            .OrderBy(e => e.Name)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var employees = entities
            .Select(e => new EmployeeDto(
                e.Name,
                e.Email,
                e.Tel,
                e.Joined.ToString("yyyy-MM-dd")))
            .ToList();

        return new PagedResult<EmployeeDto>(employees, totalCount, request.Page, request.PageSize);
    }
}
