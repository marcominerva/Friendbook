using System.Net.Mime;
using Friendbook.BusinessLayer.Services.Interfaces;
using Friendbook.DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using OperationResults;
using Entities = Friendbook.DataAccessLayer.Entities;

namespace Friendbook.BusinessLayer.Services;

public class PhotoService : IPhotoService
{
    private readonly IDbContext dbContext;

    public PhotoService(IDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<Result<ByteArrayFileContent>> GetAsync(Guid id)
    {
        var dbPerson = await dbContext.GetData<Entities.Person>().FirstOrDefaultAsync(p => p.Id == id);
        if (dbPerson?.Photo is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound);
        }

        var result = new ByteArrayFileContent(dbPerson.Photo, MediaTypeNames.Image.Jpeg);
        return result;
    }

    public async Task<Result> SaveAsync(Guid id, Stream stream)
    {
        using var photoStream = new MemoryStream();
        await stream.CopyToAsync(photoStream);
        var imageArray = photoStream.ToArray();

        // Check file header to determine if it is a valid JPEG image.
        if (imageArray is [255, 216, 255, 224, ..] or [255, 216, 255, 225, ..])
        {
            var affectedRows = await dbContext.GetData<Entities.Person>().Where(p => p.Id == id)
                .ExecuteUpdateAsync(p => p.SetProperty(x => x.Photo, x => imageArray));

            if (affectedRows == 0)
            {
                return Result.Fail(FailureReasons.ItemNotFound);
            }

            return Result.Ok();
        }

        return Result.Fail(FailureReasons.InvalidFile);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var affctedRows = await dbContext.GetData<Entities.Person>().Where(p => p.Id == id)
            .ExecuteUpdateAsync(p => p.SetProperty(x => x.Photo, x => null));

        if (affctedRows == 0)
        {
            return Result.Fail(FailureReasons.ItemNotFound);
        }

        return Result.Ok();
    }
}
