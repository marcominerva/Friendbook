using System.Net.Http.Json;
using Friendbook.Shared.Models;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Friendbook.Tests;

public class PersonApi
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public PersonApi()
    {
        //_factory = new WebApplicationFactory<Program>();
        _factory = new FriendbookFactory();
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task GetPerson_Ok()
    {
        var people = await _client.GetFromJsonAsync<ListResult<Person>>("/api/people");
        Assert.Empty(people!.Items);
    }
}