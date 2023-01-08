using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Friendbook.BusinessLayer.Services.Interfaces;
using Friendbook.DataAccessLayer;
using Friendbook.Shared.Models;
using Friendbook.Shared.Models.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OperationResults;
using TinyHelpers.Extensions;
using Entities = Friendbook.DataAccessLayer.Entities;

namespace Friendbook.BusinessLayer.Services;

internal class PeopleService : IPeopleService
{
    private readonly IDbContext dbContext;
    private readonly IMapper mapper;
    private readonly ILogger<PeopleService> logger;

    public PeopleService(IDbContext dbContext, IMapper mapper, ILogger<PeopleService> logger)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.logger = logger;
    }

    public async Task<Result<ListResult<Person>>> GetAsync(string firstName, string lastName, string city, int pageIndex, int itemsPerPage, string orderBy)
    {
        var query = dbContext.GetData<Entities.Person>()
            .WhereIf(firstName.HasValue(), p => p.FirstName.Contains(firstName))
            .WhereIf(lastName.HasValue(), p => p.LastName.Contains(lastName))
            .WhereIf(city.HasValue(), p => p.City.Contains(city));

        var totalCount = await query.CountAsync();

        if (orderBy.HasValue())
        {
            try
            {
                query = query.OrderBy(orderBy);
            }
            catch (ParseException ex)
            {
                logger.LogError(ex, "Unexpected error while trying to order by {OrderByColumn}", orderBy);
                return Result.Fail(FailureReasons.ClientError, $"Unable to order by column '{orderBy}'");
            }
        }

        var people = await query
            .Skip(pageIndex * itemsPerPage)
            .Take(itemsPerPage + 1)
            .ProjectTo<Person>(mapper.ConfigurationProvider)
            .ToListAsync();

        var result = new ListResult<Person>(people.Take(itemsPerPage), totalCount, people.Count > itemsPerPage);
        return result;
    }

    public async Task<Result<Person>> GetAsync(Guid id)
    {
        var dbPerson = await dbContext.GetData<Entities.Person>().FirstOrDefaultAsync(p => p.Id == id);
        if (dbPerson is null)
            return Result.Fail(FailureReasons.ItemNotFound);

        var person = mapper.Map<Person>(dbPerson);
        return person;
    }

    public async Task<Result<Person>> InsertAsync(SavePersonRequest request)
    {
        var samePersonExists = await dbContext.GetData<Entities.Person>()
        .AnyAsync(p => p.FirstName == request.FirstName && p.LastName == request.LastName
                && p.CreatedAt.AddMinutes(1) > DateTime.UtcNow);

        if (samePersonExists)
        {
            var validationErrors = new List<ValidationError>
                {
                    new("FirstName", "First name already in use"),
                    new("LastName", "Last name already in use")
                };

            return Result.Fail(FailureReasons.ClientError, "Unable to create a person with same first name and last name within 1 minute", validationErrors);
        }

        var dbPerson = mapper.Map<Entities.Person>(request);
        dbPerson.CreatedAt = DateTime.UtcNow;

        dbContext.Insert(dbPerson);

        await dbContext.SaveAsync();

        var person = mapper.Map<Person>(dbPerson);
        return person;
    }
}
