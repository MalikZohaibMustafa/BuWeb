namespace Bu.Web.Website.Areas.Buhs.Models;

public sealed class BuhsLayoutData : CampusLayoutData
{
    public static readonly BuhsLayoutData Data = new BuhsLayoutData();

    public override SecondaryMenuBarLinks SecondaryMenu { get; } = new SecondaryMenuBarLinks
    {
        HeaderLink = new Link
        {
            InnerHtml = "BUM&DC",
            NavigateUrl = "#",
        },
        Links = new List<NestedLink>
        {
            new NestedLink
            {
                InnerHtml = "HOD Message",
                NavigateUrl = "#",
            },
            new NestedLink
            {
                InnerHtml = "Departments",
                NavigateUrl = "#",
            },
            new NestedLink
            {
                InnerHtml = "Faculty",
                NavigateUrl = "#",
            },
            new NestedLink
            {
                InnerHtml = "Student Support Center",
                NavigateUrl = "#",
            },
            new NestedLink
            {
                InnerHtml = "Time Table",
                NavigateUrl = "#",
            },
            new NestedLink
            {
                InnerHtml = "Business Incubation Center",
                NavigateUrl = "#",
            },
            new NestedLink
            {
                InnerHtml = "Downloads",
                NavigateUrl = "#",
            },
        },
    };
}