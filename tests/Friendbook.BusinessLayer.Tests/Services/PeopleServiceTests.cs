using FluentValidation;
using Friendbook.BusinessLayer.Profiles;
using Friendbook.BusinessLayer.Services;
using Friendbook.BusinessLayer.Services.Interfaces;
using Friendbook.DataAccessLayer;
using Friendbook.DataAccessLayer.Entities;
using Friendbook.Validations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using OperationResults;

namespace Friendbook.BusinessLayer.Tests.Services;

public class PeopleServiceTests
{
    private readonly IHost host;
    private readonly Mock<IUserService> userService = new Mock<IUserService>();
    private readonly Mock<IPeopleService> peopleService = new Mock<IPeopleService>();
    private readonly Mock<ISecurityService> securityService = new Mock<ISecurityService>();
    private readonly string _databaseName = Guid.NewGuid().ToString();

    public PeopleServiceTests()
    {
        host = Host.CreateDefaultBuilder()
              .ConfigureServices((ctx, services) =>
              {
                  /*
                  //services.AddScoped<IHttpContextAccessor>(_ => new HttpContextAccessor()
                  //{
                  //    HttpContext = new DefaultHttpContext()
                  //    {
                  //        User = new System.Security.Claims.ClaimsPrincipal()
                  //    }
                  //});
                  //services.AddScoped<IUserService, HttpUserService>();
                  */

                  services.AddAutoMapper(typeof(PersonProfile).Assembly);
                  services.AddValidatorsFromAssemblyContaining<SavePersonRequestValidator>();
                  services.AddDbContext<IDbContext, ApplicationDbContext>(options =>
                  {
                      options.UseInMemoryDatabase(_databaseName);
                  });
                  services.AddScoped(_ => userService.Object);
                  services.AddScoped(_ => securityService.Object);
                  services.AddScoped<IPeopleService, PeopleService>();
              })
              .Build();
    }

    [Fact]
    public async Task Get_Guid_Ok()
    {
        var userId1 = new Guid("12E6B466-BF34-4159-B9CF-0862D9897286");
        userService.Setup(t => t.GetUserName()).Returns("Andrea Tosato");
        var service = host.Services.GetService<IPeopleService>()!;
        using var scopedContext = host.Services.CreateScope()!;
        var db = (ApplicationDbContext)(scopedContext.ServiceProvider.GetService<IDbContext>()!);
        var person1 = new Person()
        {
            Id = userId1,
            FirstName = "Andrea",
            LastName = "Tosato",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "test",
            City = "Verona",
            SecurityCode = "SEC1234"
        };
        db.Add(person1);
        await db.SaveChangesAsync();
        scopedContext.Dispose();

        var result = await service.GetAsync(userId1);
        Assert.True(result.Success);
        Assert.Equal(new Guid("12E6B466-BF34-4159-B9CF-0862D9897286"), result.Content!.Id);
        Assert.Equal("Andrea", result.Content!.FirstName);
        Assert.Equal("Tosato", result.Content!.LastName);
        Assert.Equal("Verona", result.Content!.City);
    }

    [Fact]
    public async Task Get_EmptyData_Ok()
    {
        var userId1 = new Guid("12E6B466-BF34-4159-B9CF-0862D9897286");
        userService.Setup(t => t.GetUserName()).Returns("Andrea Tosato");
        var service = host.Services.GetService<IPeopleService>()!;
        var result = await service.GetAsync(userId1);
        Assert.False(result.Success);
        Assert.Equal(FailureReasons.ItemNotFound, result.FailureReason);
    }

    [Fact]
    public async Task Get_EmptyResult_Ok()
    {
        var userId1 = new Guid("12E6B466-BF34-4159-B9CF-0862D9897286");
        userService.Setup(t => t.GetUserName()).Returns("Andrea Tosato");
        var service = host.Services.GetService<IPeopleService>()!;
        using var scopedContext = host.Services.CreateScope()!;
        var db = scopedContext.ServiceProvider.GetService<ApplicationDbContext>()!;
        var person1 = new Person()
        {
            Id = userId1,
            FirstName = "Andrea",
            LastName = "Tosato",
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "test",
            City = "Verona",
            SecurityCode = "SEC1234"
        };
        db.Add(person1);
        await db.SaveChangesAsync();
        scopedContext.Dispose();

        var result = await service.GetAsync(new Guid("12E6B466-BF34-4159-B9CF-999999999999"));
        Assert.False(result.Success);
        Assert.Equal(FailureReasons.ItemNotFound, result.FailureReason);
    }
}