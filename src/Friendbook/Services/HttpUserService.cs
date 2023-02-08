using Friendbook.BusinessLayer.Services.Interfaces;

namespace Friendbook.Services;

public class HttpUserService : IUserService
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public HttpUserService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public string GetUserName()
        => httpContextAccessor.HttpContext.User.Identity?.Name;
}
