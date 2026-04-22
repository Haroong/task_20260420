using MediatR;
using Microsoft.AspNetCore.Mvc;
using task_20260420.Common.Models;
using task_20260420.Features.Employees.Commands.AddEmployees;
using task_20260420.Features.Employees.Models;
using task_20260420.Features.Employees.Queries.GetEmployeeByName;
using task_20260420.Features.Employees.Queries.GetEmployees;
using task_20260420.Services;

namespace task_20260420.Controllers;

/// <summary>
/// 직원 연락처 API
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IContentExtractor _contentExtractor;

    public EmployeeController(IMediator mediator, IContentExtractor contentExtractor)
    {
        _mediator = mediator;
        _contentExtractor = contentExtractor;
    }

    /// <summary>
    /// 직원 목록 조회
    /// </summary>
    /// <remarks>페이지 단위로 직원 연락처 목록을 조회합니다.</remarks>
    /// <param name="page">페이지 번호 (1부터 시작, 기본값: 1)</param>
    /// <param name="pageSize">페이지당 항목 수 (1~100, 기본값: 10)</param>
    /// <response code="200">정상 조회</response>
    /// <response code="400">페이지 파라미터 오류</response>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BaseResponse<PagedResult<EmployeeDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _mediator.Send(new GetEmployeesQuery(page, pageSize));
        return Ok(BaseResponse<PagedResult<EmployeeDto>>.OnSuccess(result));
    }

    /// <summary>
    /// 직원 단건 조회
    /// </summary>
    /// <remarks>이름으로 직원 연락처 1건을 조회합니다.</remarks>
    /// <param name="name">검색할 직원 이름 (정확히 일치)</param>
    /// <response code="200">직원 정보 반환</response>
    /// <response code="404">직원을 찾을 수 없음</response>
    [HttpGet("{name}")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BaseResponse<EmployeeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByName(string name)
    {
        var result = await _mediator.Send(new GetEmployeeByNameQuery(name));
        return Ok(BaseResponse<EmployeeDto>.OnSuccess(result));
    }

    /// <summary>
    /// 직원 등록
    /// </summary>
    /// <remarks>CSV 또는 JSON 파일/텍스트로 직원 연락처를 등록합니다.</remarks>
    /// <param name="file">업로드 파일</param>
    /// <param name="data">직접 입력 데이터</param>
    /// <response code="201">직원 정보 추가 성공</response>
    /// <response code="400">입력 형식 오류</response>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(BaseResponse<AddEmployeesResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(BaseResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add(IFormFile? file, [FromForm] string? data)
    {
        var (content, fileName) = await _contentExtractor.ExtractAsync(file, data, Request);
        var result = await _mediator.Send(new AddEmployeesCommand(content, fileName));

        return Created(string.Empty, BaseResponse<AddEmployeesResult>.OnCreated(result));
    }
}
