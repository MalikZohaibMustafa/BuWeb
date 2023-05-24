using Bu.Web.Website.Models.Controls;

namespace Bu.Web.Website.Models;

public sealed class SecondaryMenuBarLinks
{
    public Link? HeaderLink { get; init; }

    public IEnumerable<NestedLink> Links { get; init; } = Array.Empty<NestedLink>();
}