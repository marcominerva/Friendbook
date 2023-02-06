using Friendbook.BusinessLayer.Services.Interfaces;

namespace Friendbook.BusinessLayer.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime GetUtcNow() => DateTime.UtcNow;
}
