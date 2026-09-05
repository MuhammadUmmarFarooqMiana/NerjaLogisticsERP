using Microsoft.Extensions.Configuration;
using NerjaLogisticsERP.Application.Common.Interfaces;

namespace NerjaLogisticsERP.Infrastructure.FileStorage;

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _rootPath;

    public LocalFileStorageService(IConfiguration configuration)
    {
        _rootPath = configuration["FileStorage:LocalRootPath"] ?? "UploadedDocuments"; //"UploadedDocuments" is outside wwwroot/UseFileServer()
        Directory.CreateDirectory(_rootPath);
    }

    public async Task<string> SaveAsync(byte[] content, string fileName, string containerName, CancellationToken cancellationToken)
    {
        var safeFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
        var containerPath = Path.Combine(_rootPath, containerName);
        Directory.CreateDirectory(containerPath);

        var fullPath = Path.Combine(containerPath, safeFileName);
        await File.WriteAllBytesAsync(fullPath, content, cancellationToken);

        return Path.Combine(containerName, safeFileName).Replace('\\', '/');
    }

    public async Task<byte[]?> GetAsync(string storageKey, CancellationToken cancellationToken)
    {
        var fullPath = Path.Combine(_rootPath, storageKey);
        return File.Exists(fullPath) ? await File.ReadAllBytesAsync(fullPath, cancellationToken) : null;
    }

    public Task DeleteAsync(string storageKey, CancellationToken cancellationToken)
    {
        var fullPath = Path.Combine(_rootPath, storageKey);
        if (File.Exists(fullPath)) File.Delete(fullPath);
        return Task.CompletedTask;
    }
}
