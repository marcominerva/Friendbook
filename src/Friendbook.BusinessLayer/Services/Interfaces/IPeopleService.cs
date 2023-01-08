using Friendbook.Shared.Models;
using Friendbook.Shared.Models.Requests;
using OperationResults;

namespace Friendbook.BusinessLayer.Services.Interfaces;

public interface IPeopleService
{
    Task<Result<ListResult<Person>>> GetAsync(string firstName, string lastName, string city, int pageIndex, int itemsPerPage, string orderBy);

    Task<Result<Person>> GetAsync(Guid id);

    Task<Result<Person>> InsertAsync(SavePersonRequest request);
}