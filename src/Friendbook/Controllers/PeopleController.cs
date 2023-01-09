using Friendbook.BusinessLayer.Services.Interfaces;
using Friendbook.Parameters;
using Friendbook.Shared.Models;
using Friendbook.Shared.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using OperationResults.AspNetCore;

namespace Friendbook.Controllers;

public class PeopleController : ControllerBase
{
    private readonly IPeopleService peopleService;

    public PeopleController(IPeopleService peopleService)
    {
        this.peopleService = peopleService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ListResult<Person>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetList([FromQuery] PeopleSearchParameters searchParameters)
    {
        var result = await peopleService.GetAsync(searchParameters.FirstName, searchParameters.LastName, searchParameters.City, searchParameters.PageIndex, searchParameters.ItemsPerPage, searchParameters.OrderBy);
        var response = HttpContext.CreateResponse(result);

        return response;
    }

    [HttpGet("{id:guid}", Name = nameof(GetPerson))]
    [ProducesResponseType(typeof(Person), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPerson(Guid id)
    {
        var result = await peopleService.GetAsync(id);
        var response = HttpContext.CreateResponse(result);

        return response;
    }

    [HttpPost]
    [ProducesResponseType(typeof(Person), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Insert(SavePersonRequest request)
    {
        var result = await peopleService.InsertAsync(request);
        var response = HttpContext.CreateResponse(result, nameof(GetPerson), new { id = result.Content?.Id });

        return response;
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, SavePersonRequest request)
    {
        var result = await peopleService.UpdateAsync(id, request);
        var response = HttpContext.CreateResponse(result);

        return response;
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await peopleService.DeleteAsync(id);
        var response = HttpContext.CreateResponse(result);

        return response;
    }
}
