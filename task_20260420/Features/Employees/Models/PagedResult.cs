namespace task_20260420.Features.Employees.Models;

/// <summary>페이지네이션 결과</summary>
/// <param name="Items">항목 목록</param>
/// <param name="TotalCount">전체 항목 수 <example>25</example></param>
/// <param name="Page">현재 페이지 <example>1</example></param>
/// <param name="PageSize">페이지당 항목 수 <example>10</example></param>
public record PagedResult<T>(
    List<T> Items,
    int TotalCount,
    int Page,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
