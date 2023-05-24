namespace Bu.Web.Website.Areas.Bulc.Models;

public sealed class BulcLayoutData : CampusLayoutData
{
    public static readonly BulcLayoutData Data = new BulcLayoutData();

    public override SecondaryMenuBarLinks SecondaryMenu { get; } = new SecondaryMenuBarLinks
    {
        HeaderLink = new Link
        {
            InnerHtml = "Lahore Campus",
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