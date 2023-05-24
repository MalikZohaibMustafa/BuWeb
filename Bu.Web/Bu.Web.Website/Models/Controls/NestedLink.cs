namespace Bu.Web.Website.Models.Controls;

public class NestedLink : Link
{
    public IEnumerable<NestedLink> ChildLinks { get; set; } = Array.Empty<NestedLink>();
}