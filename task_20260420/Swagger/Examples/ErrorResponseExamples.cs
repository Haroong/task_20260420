using Swashbuckle.AspNetCore.Filters;
using task_20260420.Common.Models;

namespace task_20260420.Swagger.Examples;

public class ErrorResponseExamples
    : IMultipleExamplesProvider<BaseResponse<object>>
{
    public IEnumerable<SwaggerExample<BaseResponse<object>>> GetExamples()
    {
        yield return SwaggerExample.Create(
            "유효성 검증 오류",
            new BaseResponse<object>(
                false, ResponseCode.ValidationError,
                "직원 데이터가 비어 있습니다.", null));

        yield return SwaggerExample.Create(
            "형식 오류",
            new BaseResponse<object>(
                false, ResponseCode.FormatError,
                "CSV 형식이 올바르지 않습니다: 헤더 행이 누락되었습니다.", null));

        yield return SwaggerExample.Create(
            "파라미터 오류",
            new BaseResponse<object>(
                false, ResponseCode.ArgumentError,
                "파일 또는 데이터를 입력해 주세요.", null));

        yield return SwaggerExample.Create(
            "직원 미발견",
            new BaseResponse<object>(
                false, ResponseCode.NotFound,
                "'홍길동' 직원을 찾을 수 없습니다.", null));

        yield return SwaggerExample.Create(
            "서버 오류",
            new BaseResponse<object>(
                false, ResponseCode.ServerError,
                "서버 내부 오류가 발생했습니다.", null));
    }
}
