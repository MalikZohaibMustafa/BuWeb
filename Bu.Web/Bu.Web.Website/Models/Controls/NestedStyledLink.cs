namespace Bu.Web.Website.Models.Controls;

public class NestedStyledLink : StyledLink
{
    public IEnumerable<NestedStyledLink> ChildLinks { get; set; } = Array.Empty<NestedStyledLink>();
}