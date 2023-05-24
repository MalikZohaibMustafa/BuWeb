using Bu.Web.Data.Abstraction;
using Bu.Web.Data.Entities;

namespace Bu.Web.Admin.Tests;

[TestClass]
public sealed class Tests03Institutes
{
    private static Lazy<Institute> GetLazyInstitute(string instituteAlias)
    {
        return new Lazy<Institute>(() =>
        {
            using IAdminContext adminContext = TestHelper.AdminContextProvider.Create();
            return adminContext.Institutes.Single(x => x.InstituteAlias == instituteAlias);
        });
    }

    public static readonly Lazy<Institute> InstituteBuho = GetLazyInstitute("BUHO");
    public static readonly Lazy<Institute> InstituteBuic = GetLazyInstitute("BUIC");
    public static readonly Lazy<Institute> InstituteBukc = GetLazyInstitute("BUKC");
    public static readonly Lazy<Institute> InstituteBulc = GetLazyInstitute("BULC");
    public static readonly Lazy<Institute> InstituteIpp = GetLazyInstitute("IPP");
    public static readonly Lazy<Institute> InstituteBuhs = GetLazyInstitute("BUMDC");
    public static readonly Lazy<Institute> InstituteNima = GetLazyInstitute("NIMA");

    [ClassInitialize]
    public static void CLassInit(TestContext context)
    {
        using IAdminContext adminContext = TestHelper.AdminContextProvider.Create();
        void AddInstitute(int id, string name, string shortName, string alias)
        {
            Institute institute = adminContext.Institutes.Add(new Institute
            {
                InstituteId = id,
                InstituteName = name,
                InstituteShortName = shortName,
                InstituteAlias = alias,
                Status = Institute.Statuses.Active,
            }).Entity;
            institute.TrackedEntity.SetCreated(TestHelper.SuperAdministratorUserNow, TestHelper.SuperAdministratorUserId);
            institute.TrackedEntity.SetLastUpdated(TestHelper.SuperAdministratorUserNow, TestHelper.SuperAdministratorUserId);
            institute.PreSave(TestHelper.SuperAdministratorClaimPrincipal);
        }

        AddInstitute(0, "Bahria University Head Office", "Head Office", "BUHO");
        AddInstitute(1, "Bahria University Islamabad Campus", "Islamabad Campus", "BUIC");
        AddInstitute(2, "Bahria University Karachi Campus", "Karachi Campus", "BUKC");
        AddInstitute(3, "Bahria University Lahore Campus", "Lahore Campus", "BULC");
        AddInstitute(5, "Bahria University Institute of Professional Psychology", "Institute of Professional Psychology", "IPP");
        AddInstitute(6, "Bahria University Medical & Dental College", "Medical & Dental College", "BUMDC");
        AddInstitute(7, "National Center for Maritime Policy Research", "National Center for Maritime Policy Research", "NIMA");
        adminContext.SaveChanges(TestHelper.SuperAdministratorClaimPrincipal);
    }

    [TestMethod]
    public void InstituteDataCheck()
    {
        Assert.IsNotNull(InstituteNima.Value);
    }
}