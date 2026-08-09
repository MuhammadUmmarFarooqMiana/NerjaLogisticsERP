namespace NerjaLogisticsERP.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> SaveAsync(byte[] content, string fileName, string containerName, CancellationToken cancellationToken);
    Task<byte[]?> GetAsync(string storageKey, CancellationToken cancellationToken);
    Task DeleteAsync(string storageKey, CancellationToken cancellationToken);
}
