using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Friendbook.BusinessLayer.Resources;
using Friendbook.BusinessLayer.Services.Interfaces;
using Friendbook.Parameters;
using Friendbook.Shared.Models;
using Friendbook.Shared.Models.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OperationResults.AspNetCore;
using TinyHelpers.AspNetCore.DataAnnotations;

namespace Friendbook.Controllers;

public class PeopleController : ControllerBase
{
    private readonly IPeopleService peopleService;
    private readonly IPhotoService photoService;

    public PeopleController(IPeopleService peopleService, IPhotoService photoService)
    {
        this.peopleService = peopleService;
        this.photoService = photoService;
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

    [Authorize]
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

    [HttpGet("{id:guid}/photo", Name = nameof(GetPhoto))]
    [Produces(contentType: MediaTypeNames.Image.Jpeg, MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPhoto(Guid id)
    {
        var result = await photoService.GetAsync(id);
        var response = HttpContext.CreateResponse(result);

        return response;
    }

    [HttpPost("{id:guid}/photo")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status415UnsupportedMediaType)]
    public async Task<IActionResult> SavePhoto(Guid id,
        [ContentType(MediaTypeNames.Image.Jpeg, ErrorMessageResourceName ="InvalidContentType", ErrorMessageResourceType =typeof(Messages))]
        [Required]
        IFormFile file)
    {
        var result = await photoService.SaveAsync(id, file.OpenReadStream());
        var response = HttpContext.CreateResponse(result, nameof(GetPhoto), new { id });

        return response;
    }

    [HttpDelete("{id:guid}/photo")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePhoto(Guid id)
    {
        var result = await photoService.DeleteAsync(id);
        var response = HttpContext.CreateResponse(result);

        return response;
    }
}