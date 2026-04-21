using MediatR;
using Microsoft.EntityFrameworkCore;
using task_20260420.Features.Employees.Models;
using task_20260420.Infrastructure;

namespace task_20260420.Features.Employees.Queries.GetEmployeeByName;

public class GetEmployeeByNameQueryHandler : IRequestHandler<GetEmployeeByNameQuery, EmployeeDto?>
{
    private readonly AppDbContext _db;

    public GetEmployeeByNameQueryHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<EmployeeDto?> Handle(GetEmployeeByNameQuery request, CancellationToken cancellationToken)
    {
        var employee = await _db.Employees
            .FirstOrDefaultAsync(e => e.Name == request.Name, cancellationToken);

        if (employee is null)
            return null;

        return new EmployeeDto(
            employee.Name,
            employee.Email,
            employee.Tel,
            employee.Joined.ToString("yyyy-MM-dd"));
    }
}
