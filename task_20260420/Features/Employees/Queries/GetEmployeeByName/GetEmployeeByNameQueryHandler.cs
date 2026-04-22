using MediatR;
using Microsoft.EntityFrameworkCore;
using task_20260420.Common.Exceptions;
using task_20260420.Features.Employees.Models;
using task_20260420.Infrastructure;

namespace task_20260420.Features.Employees.Queries.GetEmployeeByName;

public class GetEmployeeByNameQueryHandler : IRequestHandler<GetEmployeeByNameQuery, EmployeeDto>
{
    private readonly AppDbContext _db;

    public GetEmployeeByNameQueryHandler(AppDbContext db)
    {
        _db = db;
    }

    public async Task<EmployeeDto> Handle(GetEmployeeByNameQuery request, CancellationToken cancellationToken)
    {
        var employee = await _db.Employees
            .FirstOrDefaultAsync(e => e.Name == request.Name, cancellationToken);

        if (employee is null)
            throw new NotFoundException($"'{request.Name}' 직원을 찾을 수 없습니다.");

        return EmployeeDto.FromEntity(employee);
    }
}
