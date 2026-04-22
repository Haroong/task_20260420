using System.Text.Json.Serialization;

namespace task_20260420.Common.Models;

/// <summary>
/// 공통 API 응답 래퍼
/// </summary>
public record BaseResponse<T>(
    [property: JsonPropertyName("isSuccess")] bool IsSuccess,
    [property: JsonPropertyName("code")] string Code,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("result")]
    [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    T? Result)
{
    /// <summary>성공 응답 (데이터 포함)</summary>
    public static BaseResponse<T> OnSuccess(T result)
    {
        return new BaseResponse<T>(true, ResponseCode.Success, "요청이 정상 처리되었습니다.", result);
    }

    /// <summary>생성 성공 응답</summary>
    public static BaseResponse<T> OnCreated(T result)
    {
        return new BaseResponse<T>(true, ResponseCode.Created, "리소스가 정상 생성되었습니다.", result);
    }

    /// <summary>성공 응답 (데이터 미포함)</summary>
    public static BaseResponse<object?> OnSuccess()
    {
        return new BaseResponse<object?>(true, ResponseCode.Success, "요청이 정상 처리되었습니다.", default);
    }

    /// <summary>실패 응답</summary>
    public static BaseResponse<object?> OnFailure(string code, string message)
    {
        return new BaseResponse<object?>(false, code, message, default);
    }
}

public static class ResponseCode
{
    public const string Success = "COMMON200";
    public const string Created = "COMMON201";
    public const string ValidationError = "VALIDATION400";
    public const string FormatError = "FORMAT400";
    public const string ArgumentError = "ARGUMENT400";
    public const string NotFound = "NOTFOUND404";
    public const string ServerError = "SERVER500";
}
