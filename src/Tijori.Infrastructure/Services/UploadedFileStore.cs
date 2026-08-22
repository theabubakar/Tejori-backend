using System.Collections.Concurrent;
using Tijori.Application.Interfaces.Services;

namespace Tijori.Infrastructure.Services;

public class UploadedFileStore : IUploadedFileStore
{
    private readonly ConcurrentDictionary<string, UploadedFileMetadata> _files = new();

    public void Store(Guid userId, string fileToken, UploadedFileMetadata metadata)
    {
        _files[$"{userId:N}:{fileToken}"] = metadata;
    }

    public UploadedFileMetadata? Get(Guid userId, string fileToken)
    {
        _files.TryGetValue($"{userId:N}:{fileToken}", out var metadata);
        return metadata;
    }

    public void Remove(Guid userId, string fileToken)
    {
        _files.TryRemove($"{userId:N}:{fileToken}", out _);
    }
}
