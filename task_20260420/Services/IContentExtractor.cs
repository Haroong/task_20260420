namespace task_20260420.Services;

public interface IContentExtractor
{
    Task<(string Content, string? FileName)> ExtractAsync(IFormFile? file, string? data, HttpRequest request);
}
