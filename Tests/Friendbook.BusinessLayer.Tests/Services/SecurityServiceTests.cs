using Friendbook.BusinessLayer.Services;
using Friendbook.BusinessLayer.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;

namespace Friendbook.BusinessLayer.Tests.Services;
public class SecurityServiceTests
{
    private readonly IHost host;
    private readonly Mock<IDateTimeService> dataService = new();
    private readonly string _databaseName = Guid.NewGuid().ToString();

    public SecurityServiceTests()
    {
        host = Host.CreateDefaultBuilder()
            .ConfigureServices((ctx, services) =>
            {
                services.AddScoped(_ => dataService.Object);
                services.AddScoped<ISecurityService, SecurityService>();
            })
            .Build();
    }

    [Fact]
    public void GetValue()
    {
        dataService.Setup(t => t.GetUtcNow())
            .Returns(new DateTime(2023, 02, 10, 0, 0, 0));
        var securityService = host.Services.GetService<ISecurityService>()!;
        var result = securityService.GenerateHash("myinput");
        Assert.NotEmpty(result);
        Assert.Equal("A115B369B5C0F9AAD9E0CA1C065EBF38", result);
    }

    [Fact]
    public void GetValue_NoInput()
    {
        dataService.Setup(t => t.GetUtcNow())
            .Returns(new DateTime(2023, 02, 10, 0, 0, 0));
        var securityService = host.Services.GetService<ISecurityService>()!;
        var result = securityService.GenerateHash(null);
        Assert.NotEmpty(result);
        Assert.Equal("754AB3DEDA9BE5BADC58BC1E68E680E8", result);
    }
}
