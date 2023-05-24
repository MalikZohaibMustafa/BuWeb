using Bu.Web.Data.Abstraction;
using Bu.Web.Data.Entities;

namespace Bu.Web.Admin.Tests;

[TestClass]
public sealed class Tests06Areas
{
    private static Lazy<Area> GetLazyArea(int instiuteId, int? instituteLocationId, int? departmentId)
    {
        return new Lazy<Area>(() =>
        {
            using IAdminContext adminContext = TestHelper.AdminContextProvider.Create();
            return adminContext.Areas.Single(x => x.InstituteId == instiuteId && x.InstituteLocationId == instituteLocationId && x.DepartmentId == departmentId);
        });
    }

    public static readonly Lazy<Area> AreaBuho = GetLazyArea(Tests03Institutes.InstituteBuho.Value.Id, null, null);
    public static readonly Lazy<Area> AreaBuic = GetLazyArea(Tests03Institutes.InstituteBuic.Value.Id, null, null);
    public static readonly Lazy<Area> AreaBukc = GetLazyArea(Tests03Institutes.InstituteBukc.Value.Id, null, null);
    public static readonly Lazy<Area> AreaBulc = GetLazyArea(Tests03Institutes.InstituteBulc.Value.Id, null, null);
    public static readonly Lazy<Area> AreaIpp = GetLazyArea(Tests03Institutes.InstituteIpp.Value.Id, null, null);
    public static readonly Lazy<Area> AreaBuhs = GetLazyArea(Tests03Institutes.InstituteBuhs.Value.Id, null, null);
    public static readonly Lazy<Area> AreaNima = GetLazyArea(Tests03Institutes.InstituteNima.Value.Id, null, null);

    [ClassInitialize]
    public static void CLassInit(TestContext context)
    {
        using IAdminContext adminContext = TestHelper.AdminContextProvider.Create();
        void AddArea(Area? parentArea, string areaName, Institute institute, InstituteLocation? instituteLocation, Department? department)
        {
            Area area = adminContext.Areas.Add(new Area
            {
                ParentAreaId = parentArea?.Id,
                AreaName = areaName,
                AreaPath = parentArea == null ? Area.RootPath : parentArea.AreaPath + areaName,
                InstituteId = institute.InstituteId,
                InstituteLocationId = instituteLocation?.InstituteLocationId,
                DepartmentId = department?.DepartmentId,
                Status = Area.Statuses.Active,
                ContactOffice = "IT Directorate BUHO",
            }).Entity;
            area.TrackedEntity.SetCreated(TestHelper.SuperAdministratorUserNow, TestHelper.SuperAdministratorUserId);
            area.TrackedEntity.SetLastUpdated(TestHelper.SuperAdministratorUserNow, TestHelper.SuperAdministratorUserId);
            area.PreSave(TestHelper.SuperAdministratorClaimPrincipal);
            adminContext.SaveChanges(TestHelper.SuperAdministratorClaimPrincipal);
        }

        AddArea(null, string.Empty, Tests03Institutes.InstituteBuho.Value, null, null);
        AddArea(AreaBuho.Value, "buic", Tests03Institutes.InstituteBuic.Value, null, null);
        AddArea(AreaBuho.Value, "bukc", Tests03Institutes.InstituteBukc.Value, null, null);
        AddArea(AreaBuho.Value, "bulc", Tests03Institutes.InstituteBulc.Value, null, null);
        AddArea(AreaBuho.Value, "ipp", Tests03Institutes.InstituteIpp.Value, null, null);
        AddArea(AreaBuho.Value, "buhs", Tests03Institutes.InstituteBuhs.Value, null, null);
        AddArea(AreaBuho.Value, "nima", Tests03Institutes.InstituteNima.Value, null, null);
    }

    [TestMethod]
    public void AreaDataCheck()
    {
        Assert.IsNotNull(AreaNima.Value);
    }
}