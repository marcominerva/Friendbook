using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Friendbook.BusinessLayer.Services.Interfaces;

namespace Friendbook.BusinessLayer.Services;

internal class SecurityService : ISecurityService
{
    private readonly IDateTimeService dateTimeService;

    public SecurityService(IDateTimeService dateTimeService)
    {
        this.dateTimeService = dateTimeService;
    }

    public string GenerateHash(string input)
    {
        // https://passwordsgenerator.net/md5-hash-generator/
        var data = $"{input}-{dateTimeService.GetUtcNow().Ticks}";
        var bytes = Encoding.UTF8.GetBytes(data);
        var hash = MD5.HashData(bytes);

        return Convert.ToHexString(hash);
    }
}
