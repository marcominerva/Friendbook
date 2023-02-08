using System.Diagnostics.CodeAnalysis;

namespace Friendbook.DataAccessLayer;

[ExcludeFromCodeCoverage]
public class ApplicationDbContextOptions
{
    public string ConnectionString { get; set; }
}
