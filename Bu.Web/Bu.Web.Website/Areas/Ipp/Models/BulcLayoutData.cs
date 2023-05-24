namespace Bu.Web.Website.Areas.Ipp.Models;

public sealed class IppLayoutData : CampusLayoutData
{
    public static readonly IppLayoutData Data = new IppLayoutData();

    public override SecondaryMenuBarLinks SecondaryMenu { get; } = new SecondaryMenuBarLinks
    {
        HeaderLink = new Link
        {
            InnerHtml = "IPP",
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