namespace Bu.Web.Website.Models;

public static class BuData
{
    public static readonly TopBarLinks TopBarLinks = new TopBarLinks
    {
        Buttons = new List<Link>
        {
            new Link
            {
                InnerHtml = $"Apply Now {(Icon)"bi-arrow-right-circle-fill"}",
                NavigateUrl = "#",
            },
        },
        Links = new List<Link>
        {
            new Link
            {
                InnerHtml = "Scholarships",
                NavigateUrl = "#",
            },
            new Link
            {
                InnerHtml = "Giving (Endowment)",
                NavigateUrl = "#",
            },
            new Link
            {
                InnerHtml = "Downloads",
                NavigateUrl = "#",
            },
            new Link
            {
                InnerHtml = "Jobs",
                NavigateUrl = "#",
            },
            new Link
            {
                InnerHtml = "Tenders",
                NavigateUrl = "#",
            },
            new Link
            {
                InnerHtml = "Press Releases",
                NavigateUrl = "#",
            },
            new Link
            {
                InnerHtml = "WebMail",
                NavigateUrl = "#",
            },
            new Link
            {
                InnerHtml = "Office Directory",
                NavigateUrl = "~/Home/OfficeDirectory",
            },
            new Link
            {
                InnerHtml = "Contacts",
                NavigateUrl = "~/Home/ContactUs",
            },
        },
        SearchDomainUrl = "~",
        SearchEnabled = true,
    };

    public static readonly LogoBar LogoBar = new LogoBar
    {
        LogoUrl = "~/images/bu/bu-logo.png",
        NavigateUrl = "~/",
    };

    public static readonly IEnumerable<NestedLink> MainMenuLinks = new[]
    {
        new NestedLink
        {
            InnerHtml = "Home",
            NavigateUrl = "~/",
        },
        new NestedLink
        {
            InnerHtml = "About Us",
            NavigateUrl = "#",
            ChildLinks = new[]
            {
                new NestedLink
                {
                    InnerHtml = "Rector's Welcome Message",
                    NavigateUrl = "~/About/RectorMessage",
                },
                new NestedLink
                {
                    InnerHtml = "Director General's Message",
                    NavigateUrl = "#",
                    ChildLinks = new[]
                    {
                        new NestedLink
                        {
                            InnerHtml = "Islamabad Campus",
                            NavigateUrl = "~/Home",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Lahore Campus",
                            NavigateUrl = "~/Home",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Karachi Campus",
                            NavigateUrl = "~/Home",
                        },
                    },
                },
                new NestedLink
                {
                    InnerHtml = "Why Bahria?",
                    NavigateUrl = "~/About/WhyBahria",
                },
                new NestedLink
                {
                    InnerHtml = "Mission &amp; Vision",
                    NavigateUrl = "~/About/VisionAndMission",
                },
                new NestedLink
                {
                    InnerHtml = "Board of Governors",
                    NavigateUrl = "~/About/BoardOfGovernors",
                },
                new NestedLink
                {
                    InnerHtml = "Office Directory",
                    NavigateUrl = "~/About/OfficeDirectory",
                },
                new NestedLink
                {
                    InnerHtml = "Contact Us",
                    NavigateUrl = "~/About/ContactUs",
                },
            },
        },
        new NestedLink
        {
            InnerHtml = "Directorates",
            NavigateUrl = "#",
            ChildLinks = new[]
            {
                new NestedLink
                {
                    InnerHtml = "Admissions",
                    NavigateUrl = "~/Directorate/Admissions",
                },
                new NestedLink
                {
                    InnerHtml = "Academics",
                    NavigateUrl = "#",
                    ChildLinks = new[]
                    {
                        new NestedLink
                        {
                            InnerHtml = "Introduction",
                            NavigateUrl = "~/Directorate/Academics/Introduction",
                            ChildLinks = new[]
                            {
                                new NestedLink
                                {
                                    InnerHtml = "abc",
                                    NavigateUrl = "#",
                                },
                            },
                        },
                        new NestedLink
                        {
                            InnerHtml = "Director's Message",
                            NavigateUrl = "~/Directorate/Academics/DirectorMessage",
                        },
                    },
                },
                new NestedLink
                {
                    InnerHtml = "PGP Directorate",
                    NavigateUrl = "~/Directorate/PG",
                },
                new NestedLink
                {
                    InnerHtml = "Human Resource",
                    NavigateUrl = "~/Directorate/HR",
                },
                new NestedLink
                {
                    InnerHtml = "IT Directorate",
                    NavigateUrl = "~/Directorate/IT",
                },
                new NestedLink
                {
                    InnerHtml = "Leadership Development Center",
                    NavigateUrl = "~/Directorate/LDC",
                },
                new NestedLink
                {
                    InnerHtml = "RIC",
                    LinkTarget = LinkTargets.Blank,
                    NavigateUrl = "https://www.bahria.edu.pk/oric",
                },
                new NestedLink
                {
                    InnerHtml = "BU Advancement",
                    NavigateUrl = "~/Directorate/BUAD",
                },
                new NestedLink
                {
                    InnerHtml = "National Institute of Maritime Affairs",
                    LinkTarget = LinkTargets.Blank,
                    NavigateUrl = "https://bahria.edu.pk/nima/",
                },
                new NestedLink
                {
                    InnerHtml = "Quality Assurance",
                    LinkTarget = LinkTargets.Blank,
                    NavigateUrl = "https://www.bahria.edu.pk/qa/",
                },
                new NestedLink
                {
                    InnerHtml = "Student Affairs",
                    NavigateUrl = "~/Directorate/DSA",
                },
            },
        },
        new NestedLink
        {
            InnerHtml = "Campuses",
            NavigateUrl = "~/Home",
            ChildLinks = new[]
            {
                new NestedLink()
                {
                    InnerHtml = "Islamabad Campus",
                    LinkTarget = LinkTargets.Blank,
                    NavigateUrl = "~/Buic",
                },
                new NestedLink()
                {
                    InnerHtml = "Karachi Campus",
                    LinkTarget = LinkTargets.Blank,
                    NavigateUrl = "~/Bukc",
                },
                new NestedLink()
                {
                    InnerHtml = "Lahore Campus",
                    LinkTarget = LinkTargets.Blank,
                    NavigateUrl = "~/Bulc",
                },
                new NestedLink()
                {
                    InnerHtml = "Bahria University Health Sciences",
                    LinkTarget = LinkTargets.Blank,
                    NavigateUrl = "~/Buhs",
                },
                new NestedLink()
                {
                    InnerHtml = "Institute of Professional Psychology",
                    LinkTarget = LinkTargets.Blank,
                    NavigateUrl = "~/Ipp",
                },
                new NestedLink()
                {
                    InnerHtml = "National Institute of Maritime Affairs",
                    LinkTarget = LinkTargets.Blank,
                    NavigateUrl = "https://www.bahria.edu.pk/nima",
                },
                new NestedLink()
                {
                    InnerHtml = "National Centre for Maritime Policy Research",
                    LinkTarget = LinkTargets.Blank,
                    NavigateUrl = "https://www.bahria.edu.pk/ncmpr",
                },
            },
        },
        new NestedLink
        {
            InnerHtml = "Academics",
            NavigateUrl = "#",
            ChildLinks = new[]
            {
                new NestedLink()
                {
                    InnerHtml = "Faculty",
                    NavigateUrl = "~/Faculty/",
                },
                new NestedLink()
                {
                    InnerHtml = "Program / Roadmap",
                    NavigateUrl = "~/Programs/",
                    ChildLinks = new[]
                    {
                        new NestedLink
                        {
                            InnerHtml = "Undergraduate Programs",
                            NavigateUrl = "~/Programs/UndergraduatePrograms",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Graduate/Postgraduate Programs",
                            NavigateUrl = "~/Programs/UndergraduatePrograms",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Doctorate Programs",
                            NavigateUrl = "~/Programs/DoctoratePrograms",
                        },
                    },
                },
            },
        },
        new NestedLink
        {
            InnerHtml = "Admissions",
            NavigateUrl = "#",
            ChildLinks = new[]
            {
                new NestedLink()
                {
                    InnerHtml = "Eligibility Criteria",
                    NavigateUrl = "#",
                    ChildLinks = new[]
                    {
                        new NestedLink
                        {
                            InnerHtml = "Undergraduate Programs",
                            NavigateUrl = "#",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Graduate/Postgraduate Programs",
                            NavigateUrl = "#",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Doctorate Programs",
                            NavigateUrl = "#",
                        },
                    },
                },
            },
        },
        new NestedLink
        {
            InnerHtml = "Research",
            NavigateUrl = "https://www.bahria.edu.pk/oric/",
            LinkTarget = LinkTargets.Blank,
        },
        new NestedLink
        {
            InnerHtml = "On Campus",
            NavigateUrl = "#",
            ChildLinks = new[]
            {
                new NestedLink()
                {
                    InnerHtml = "News & Events",
                    NavigateUrl = "#",
                },
                new NestedLink()
                {
                    InnerHtml = "Facilities",
                    NavigateUrl = "#",
                    ChildLinks = new[]
                    {
                        new NestedLink
                        {
                            InnerHtml = "International Linkage",
                            NavigateUrl = "~/OnCampus/Facilities/InternationalLinkage",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Health Care Discounts",
                            NavigateUrl = "~/OnCampus/Facilities/HealthCareDiscounts",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Microsoft DreamSpark",
                            NavigateUrl = "https://bahriau.onthehub.com/WebStore/Welcome.aspx?vsro=8",
                            LinkTarget = LinkTargets.Blank,
                        },
                        new NestedLink
                        {
                            InnerHtml = "Hostel Facility",
                            NavigateUrl = "~/OnCampus/Facilities/HostelFacility",
                        },
                        new NestedLink
                        {
                            InnerHtml = "IT Services",
                            NavigateUrl = "~/OnCampus/Facilities/ITServices",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Medical Facility",
                            NavigateUrl = "~/OnCampus/Facilities/Medical",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Gymnasium",
                            NavigateUrl = "~/OnCampus/Facilities/Gymnasium",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Pern Facility",
                            NavigateUrl = "~/OnCampus/Facilities/PernFacility",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Media Lab",
                            NavigateUrl = "~/OnCampus/Facilities/MediaLab",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Geophysics Lab",
                            NavigateUrl = "~/OnCampus/Facilities/GeophysicsLab",
                        },
                    },
                },
                new NestedLink()
                {
                    InnerHtml = "Scholarships",
                    NavigateUrl = "https://www.bahria.edu.pk/scholarships/",
                    LinkTarget = LinkTargets.Blank,
                },
                new NestedLink()
                {
                    InnerHtml = "Student Loan Scheme",
                    NavigateUrl = "#",
                },
                new NestedLink()
                {
                    InnerHtml = "Dress Code",
                    NavigateUrl = "#",
                },
                new NestedLink()
                {
                    InnerHtml = "Issued Degrees",
                    NavigateUrl = "#",
                },
                new NestedLink()
                {
                    InnerHtml = "Convocations",
                    NavigateUrl = "#",
                },
                new NestedLink()
                {
                    InnerHtml = "Student Profile Login",
                    NavigateUrl = "#",
                },
            },
        },
        new NestedLink
        {
            InnerHtml = "Downloads",
            NavigateUrl = "#",
        },
        new NestedLink
        {
            InnerHtml = "Alumni",
            NavigateUrl = "#",
            ChildLinks = new[]
            {
                new NestedLink()
                {
                    InnerHtml = "Islamabad",
                    NavigateUrl = "#",
                    ChildLinks = new[]
                    {
                        new NestedLink
                        {
                            InnerHtml = "Assistant Manager Alumni Affairs",
                            NavigateUrl = "~/Buic/Alumni",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Alumni Featured Stories",
                            NavigateUrl = "~/Buic/Alumni/FeaturedStories",
                        },
                        new NestedLink
                        {
                            InnerHtml = "All BUAA Details",
                            NavigateUrl = "~/Buic/Alumni/AllBuaaDetails",
                        },
                        new NestedLink
                        {
                            InnerHtml = "5th BUAA Members",
                            NavigateUrl = "~/Buic/Alumni/CurrentBuaaMembers",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Alumni Newsletter",
                            NavigateUrl = "~/Buic/Alumni/NewsLetter",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Alumni Card Registration",
                            NavigateUrl = "~/Buic/Alumni/CardRegistration",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Alumni Activities",
                            NavigateUrl = "~/Buic/Alumni/Activities",
                        },
                        new NestedLink
                        {
                            InnerHtml = "BUAA Profiles - BUIC",
                            NavigateUrl = "~/Buic/Alumni/BuaaProfiles",
                        },
                        new NestedLink
                        {
                            InnerHtml = "BUTV - Back to Bahria",
                            NavigateUrl = "~/Buic/Alumni/butv",
                        },
                    },
                },
                new NestedLink()
                {
                    InnerHtml = "Lahore",
                    NavigateUrl = "#",
                    ChildLinks = new[]
                    {
                        new NestedLink
                        {
                            InnerHtml = "Placement Officer",
                            NavigateUrl = "~/Bulc/Alumni",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Alumni Featured Stories",
                            NavigateUrl = "~/Bulc/Alumni/FeaturedStories",
                        },
                        new NestedLink
                        {
                            InnerHtml = "BUAA Profiles - BULC",
                            NavigateUrl = "~/Bulc/Alumni/BuaaProfiles",
                        },
                    },
                },
                new NestedLink()
                {
                    InnerHtml = "Karachi",
                    NavigateUrl = "#",
                    ChildLinks = new[]
                    {
                        new NestedLink
                        {
                            InnerHtml = "Assistant Manager Alumni Affairs",
                            NavigateUrl = "~/Bukc/Alumni",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Alumni Featured Stories",
                            NavigateUrl = "~/Bukc/Alumni/FeaturedStories",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Alumni Activities",
                            NavigateUrl = "~/Bukc/Alumni/Activities",
                        },
                        new NestedLink
                        {
                            InnerHtml = "BUAA Profiles - BUIC",
                            NavigateUrl = "~/Bukc/Alumni/BuaaProfiles",
                        },
                    },
                },
                new NestedLink()
                {
                    InnerHtml = "BUHS",
                    NavigateUrl = "#",
                    ChildLinks = new[]
                    {
                        new NestedLink
                        {
                            InnerHtml = "Manager SSC",
                            NavigateUrl = "~/Buhs/Alumni",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Alumni Featured Stories",
                            NavigateUrl = "~/Buhs/Alumni/FeaturedStories",
                        },
                    },
                },
                new NestedLink()
                {
                    InnerHtml = "IPP",
                    NavigateUrl = "#",
                    ChildLinks = new[]
                    {
                        new NestedLink
                        {
                            InnerHtml = "Alumni Coordinator",
                            NavigateUrl = "~/Ipp/Alumni",
                        },
                        new NestedLink
                        {
                            InnerHtml = "Alumni Featured Stories",
                            NavigateUrl = "~/Ipp/Alumni/FeaturedStories",
                        },
                    },
                },
            },
        },
    };
}