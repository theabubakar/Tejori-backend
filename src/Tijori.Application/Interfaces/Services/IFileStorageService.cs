using Tijori.Application.Options;

namespace Tijori.Application.Interfaces.Services;

public interface IFileStorageService
{
    Task<(string StoredFileName, long SizeBytes)> SaveUserFileAsync(
        Guid userId,
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default);

    Task<(Stream FileStream, string ContentType, string FileName)?> OpenUserFileAsync(
        Guid userId,
        string storedFileName,
        CancellationToken cancellationToken = default);

    Task DeleteUserFileAsync(Guid userId, string storedFileName, CancellationToken cancellationToken = default);
}

public interface IUploadedFileStore
{
    void Store(Guid userId, string fileToken, UploadedFileMetadata metadata);
    UploadedFileMetadata? Get(Guid userId, string fileToken);
    void Remove(Guid userId, string fileToken);
}

public class UploadedFileMetadata
{
    public string StoredFileName { get; init; } = string.Empty;
    public string FileName { get; init; } = string.Empty;
    public string ContentType { get; init; } = string.Empty;
    public long FileSizeBytes { get; init; }
}
