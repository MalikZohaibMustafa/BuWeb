using Bu.Web.Data.Abstraction;
using Bu.Web.Data.Entities;

namespace Bu.Web.Admin.Tests;

[TestClass]
public sealed class Tests04InstituteLocations
{
    private static Lazy<InstituteLocation> GetLazyInstituteLocation(string locationAlias)
    {
        return new Lazy<InstituteLocation>(() =>
        {
            using IAdminContext adminContext = TestHelper.AdminContextProvider.Create();
            return adminContext.InstituteLocations.Single(x => x.LocationAlias == locationAlias);
        });
    }

    public static readonly Lazy<InstituteLocation> InstituteLocationE8Buic = GetLazyInstituteLocation("E8");
    public static readonly Lazy<InstituteLocation> InstituteLocationH11Buic = GetLazyInstituteLocation("H11");

    [ClassInitialize]
    public static void CLassInit(TestContext context)
    {
        using IAdminContext adminContext = TestHelper.AdminContextProvider.Create();
        void AddInstituteLocation(int instituteId, string name, string shortName, string alias)
        {
            InstituteLocation instituteLocation = adminContext.InstituteLocations.Add(new InstituteLocation
            {
                InstituteId = instituteId,
                LocationName = name,
                LocationShortName = shortName,
                LocationAlias = alias,
                Status = InstituteLocation.Statuses.Active,
            }).Entity;
            instituteLocation.TrackedEntity.SetCreated(TestHelper.SuperAdministratorUserNow, TestHelper.SuperAdministratorUserId);
            instituteLocation.TrackedEntity.SetLastUpdated(TestHelper.SuperAdministratorUserNow, TestHelper.SuperAdministratorUserId);
            instituteLocation.PreSave(TestHelper.SuperAdministratorClaimPrincipal);
        }

        AddInstituteLocation(Tests03Institutes.InstituteBuic.Value.Id, "E-8", "E-8", "E8");
        AddInstituteLocation(Tests03Institutes.InstituteBuic.Value.Id, "H-11", "H-11", "H11");
        adminContext.SaveChanges(TestHelper.SuperAdministratorClaimPrincipal);
    }

    [TestMethod]
    public void InstituteLocationDataCheck()
    {
        Assert.IsNotNull(InstituteLocationH11Buic.Value);
    }
}