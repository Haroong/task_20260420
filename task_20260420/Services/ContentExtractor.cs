using System.Text;
using task_20260420.Common.Exceptions;

namespace task_20260420.Services;

public class ContentExtractor : IContentExtractor
{
    public async Task<(string Content, string? FileName)> ExtractAsync(
        IFormFile? file, string? data, HttpRequest request)
    {
        // 1순위: 파일 업로드 (input type=file)
        if (file is not null && file.Length > 0)
        {
            using var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8);
            var content = await reader.ReadToEndAsync();
            return (content, file.FileName);
        }

        // 2순위: 폼 텍스트 필드 (textarea)
        if (!string.IsNullOrWhiteSpace(data))
            return (data, null);

        // 3순위: raw body (폼이 아닌 직접 본문 전송)
        if (!request.HasFormContentType)
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8);
            var content = await reader.ReadToEndAsync();

            if (!string.IsNullOrWhiteSpace(content))
                return (content, null);
        }

        throw new InvalidArgumentException("파일 또는 데이터를 입력해 주세요.");
    }
}
