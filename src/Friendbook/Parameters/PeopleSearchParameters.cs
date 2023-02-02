using Microsoft.AspNetCore.Mvc;

namespace Friendbook.Parameters;

public class PeopleSearchParameters
{
    [FromQuery(Name = "firstName")]
    public string FirstName { get; set; }

    [FromQuery(Name = "lastName")]
    public string LastName { get; set; }

    [FromQuery(Name = "city")]
    public string City { get; set; }

    [FromQuery(Name = "pageIndex")]
    public int PageIndex { get; set; }

    [FromQuery(Name = "itemsPerPage")]
    public int ItemsPerPage { get; set; } = 50;

    [FromQuery(Name = "orderBy")]
    public string OrderBy { get; set; } = "FirstName ASC, LastName ASC";
}
