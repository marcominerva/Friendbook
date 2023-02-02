using OperationResults;

namespace Friendbook.BusinessLayer.Services.Interfaces;

public interface IPhotoService
{
    Task<Result<ByteArrayFileContent>> GetAsync(Guid id);

    Task<Result> SaveAsync(Guid id, Stream stream);

    Task<Result> DeleteAsync(Guid id);
}