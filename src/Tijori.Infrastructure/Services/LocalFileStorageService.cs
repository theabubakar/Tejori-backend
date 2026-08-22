using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Tijori.Application.Interfaces.Services;
using Tijori.Application.Options;

namespace Tijori.Infrastructure.Services;

public class LocalFileStorageService : IFileStorageService
{
    private readonly StorageSettings _settings;
    private readonly IHostEnvironment _environment;

    public LocalFileStorageService(IOptions<StorageSettings> settings, IHostEnvironment environment)
    {
        _settings = settings.Value;
        _environment = environment;
    }

    public async Task<(string StoredFileName, long SizeBytes)> SaveUserFileAsync(
        Guid userId,
        Stream fileStream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var safeFileName = Path.GetFileName(fileName);
        var storedFileName = $"{Guid.NewGuid():N}_{safeFileName}";
        var userDirectory = Path.Combine(
            _environment.ContentRootPath,
            _settings.UploadRootPath,
            userId.ToString("N"));

        Directory.CreateDirectory(userDirectory);

        var fullPath = Path.Combine(userDirectory, storedFileName);
        await using var output = File.Create(fullPath);
        await fileStream.CopyToAsync(output, cancellationToken);
        var sizeBytes = output.Length;
        return (storedFileName, sizeBytes);
    }

    public Task<(Stream FileStream, string ContentType, string FileName)?> OpenUserFileAsync(
        Guid userId,
        string storedFileName,
        CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(
            _environment.ContentRootPath,
            _settings.UploadRootPath,
            userId.ToString("N"),
            Path.GetFileName(storedFileName));

        if (!File.Exists(fullPath))
        {
            return Task.FromResult<(Stream, string, string)?>(null);
        }

        Stream stream = File.OpenRead(fullPath);
        return Task.FromResult<(Stream, string, string)?>((stream, "application/octet-stream", storedFileName));
    }

    public Task DeleteUserFileAsync(Guid userId, string storedFileName, CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(
            _environment.ContentRootPath,
            _settings.UploadRootPath,
            userId.ToString("N"),
            Path.GetFileName(storedFileName));

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}
