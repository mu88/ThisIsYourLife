using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Logging.Extensions;

[ExcludeFromCodeCoverage]
public static class Tracing
{
    public static readonly ActivitySource Source = new("ThisIsYourLife");
}
