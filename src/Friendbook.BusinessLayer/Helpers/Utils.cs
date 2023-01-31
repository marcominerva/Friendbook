using System.Security.Cryptography;
using System.Text;

namespace Friendbook.BusinessLayer.Helpers;

internal static class Utils
{
    public static string GenerateHash(string input)
    {
        // https://passwordsgenerator.net/md5-hash-generator/
        var data = $"{input}-{DateTime.UtcNow.Ticks}";
        var bytes = Encoding.UTF8.GetBytes(data);
        var hash = MD5.HashData(bytes);

        return Convert.ToHexString(hash);
    }
}
