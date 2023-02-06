﻿using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Friendbook.BusinessLayer.Resources;
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
    private readonly IUserService userService;
    private readonly IValidator<SavePersonRequest> validator;
    private readonly ISecurityService securityService;
    private readonly ILogger<PeopleService> logger;

    public PeopleService(IDbContext dbContext, IMapper mapper, IUserService userService, IValidator<SavePersonRequest> validator, ISecurityService securityService,
        ILogger<PeopleService> logger)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
        this.userService = userService;
        this.validator = validator;
        this.securityService = securityService;
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
                logger.LogError(ex, Messages.UnexpectedOrderByErrorMessage, orderBy);
                return Result.Fail(FailureReasons.ClientError, string.Format(Messages.UnableToSort, orderBy));
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
        {
            return Result.Fail(FailureReasons.ItemNotFound);
        }

        var person = mapper.Map<Person>(dbPerson);
        return person;
    }

    public async Task<Result<Person>> InsertAsync(SavePersonRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var validationErrors = validationResult.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
            return Result.Fail(FailureReasons.ClientError, validationErrors);
        }

        var samePersonExists = await dbContext.GetData<Entities.Person>()
            .AnyAsync(p => p.FirstName == request.FirstName && p.LastName == request.LastName
            && p.CreatedAt.AddMinutes(1) > DateTime.UtcNow);

        if (samePersonExists)
        {
            var validationErrors = new List<ValidationError>
            {
                new(nameof(Person.FirstName), Messages.FirstNameInUse),
                new(nameof(Person.LastName), Messages.LastNameInUse)
            };

            return Result.Fail(FailureReasons.ClientError, Messages.UnableToCreatePersonWithinOneMinute, validationErrors);
        }

        var dbPerson = mapper.Map<Entities.Person>(request);

        dbPerson.SecurityCode = securityService.GenerateHash($"{dbPerson.FirstName}{dbPerson.LastName}");
        dbPerson.CreatedAt = DateTime.UtcNow;
        dbPerson.CreatedBy = userService.GetUserName();

        dbContext.Insert(dbPerson);

        await dbContext.SaveAsync();

        var person = mapper.Map<Person>(dbPerson);
        return person;
    }

    public async Task<Result> UpdateAsync(Guid id, SavePersonRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var validationErrors = validationResult.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage));
            return Result.Fail(FailureReasons.ClientError, validationErrors);
        }

        var dbPerson = await dbContext.GetData<Entities.Person>(trackingChanges: true).FirstOrDefaultAsync(p => p.Id == id);
        if (dbPerson is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound);
        }

        mapper.Map(request, dbPerson);
        dbPerson.SecurityCode = securityService.GenerateHash($"{dbPerson.FirstName}{dbPerson.LastName}");

        await dbContext.SaveAsync();

        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var affctedRows = await dbContext.GetData<Entities.Person>().Where(p => p.Id == id).ExecuteDeleteAsync();
        if (affctedRows == 0)
        {
            return Result.Fail(FailureReasons.ItemNotFound);
        }

        return Result.Ok();
    }
}
