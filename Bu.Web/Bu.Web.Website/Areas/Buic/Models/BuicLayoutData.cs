namespace Bu.Web.Website.Areas.Buic.Models;

public sealed class BuicLayoutData : CampusLayoutData
{
    public static readonly BuicLayoutData Data = new BuicLayoutData();

    public override SecondaryMenuBarLinks SecondaryMenu { get; } = new SecondaryMenuBarLinks
    {
        HeaderLink = new Link
        {
            InnerHtml = "Islamabad Campus",
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