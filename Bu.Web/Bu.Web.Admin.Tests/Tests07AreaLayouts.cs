using Bu.AspNetCore.Core.Extensions;
using Bu.Web.Data.Abstraction;
using Bu.Web.Data.Entities;

namespace Bu.Web.Admin.Tests;

[TestClass]
public sealed class Tests07AreaLayouts
{
    private static Lazy<AreaLayout> GetLazyAreaLayout(int areaId, AreaLayout.LayoutTypes layoutType)
    {
        return new Lazy<AreaLayout>(() =>
        {
            using IAdminContext adminContext = TestHelper.AdminContextProvider.Create();
            return adminContext.AreaLayouts.Single(x => x.AreaId == areaId && x.LayoutType == layoutType);
        });
    }

    public static readonly Lazy<AreaLayout> AreaLayoutBuho = GetLazyAreaLayout(Tests06Areas.AreaBuho.Value.Id, AreaLayout.LayoutTypes.NoMenu);
    public static readonly Lazy<AreaLayout> AreaLayoutBuhoLeftMenu = GetLazyAreaLayout(Tests06Areas.AreaBuho.Value.Id, AreaLayout.LayoutTypes.LeftMenu);
    public static readonly Lazy<AreaLayout> AreaLayoutBuhoRightMenu = GetLazyAreaLayout(Tests06Areas.AreaBuho.Value.Id, AreaLayout.LayoutTypes.RightMenu);
    public static readonly Lazy<AreaLayout> AreaLayoutBuic = GetLazyAreaLayout(Tests06Areas.AreaBuic.Value.Id, AreaLayout.LayoutTypes.NoMenu);
    public static readonly Lazy<AreaLayout> AreaLayoutBuicLeftMenu = GetLazyAreaLayout(Tests06Areas.AreaBuic.Value.Id, AreaLayout.LayoutTypes.LeftMenu);
    public static readonly Lazy<AreaLayout> AreaLayoutBuicRightMenu = GetLazyAreaLayout(Tests06Areas.AreaBuic.Value.Id, AreaLayout.LayoutTypes.RightMenu);
    public static readonly Lazy<AreaLayout> AreaLayoutBukc = GetLazyAreaLayout(Tests06Areas.AreaBukc.Value.Id, AreaLayout.LayoutTypes.NoMenu);
    public static readonly Lazy<AreaLayout> AreaLayoutBukcLeftMenu = GetLazyAreaLayout(Tests06Areas.AreaBukc.Value.Id, AreaLayout.LayoutTypes.LeftMenu);
    public static readonly Lazy<AreaLayout> AreaLayoutBukcRightMenu = GetLazyAreaLayout(Tests06Areas.AreaBukc.Value.Id, AreaLayout.LayoutTypes.RightMenu);
    public static readonly Lazy<AreaLayout> AreaLayoutBulc = GetLazyAreaLayout(Tests06Areas.AreaBulc.Value.Id, AreaLayout.LayoutTypes.NoMenu);
    public static readonly Lazy<AreaLayout> AreaLayoutBulcLeftMenu = GetLazyAreaLayout(Tests06Areas.AreaBulc.Value.Id, AreaLayout.LayoutTypes.LeftMenu);
    public static readonly Lazy<AreaLayout> AreaLayoutBulcRightMenu = GetLazyAreaLayout(Tests06Areas.AreaBulc.Value.Id, AreaLayout.LayoutTypes.RightMenu);
    public static readonly Lazy<AreaLayout> AreaLayoutIpp = GetLazyAreaLayout(Tests06Areas.AreaIpp.Value.Id, AreaLayout.LayoutTypes.NoMenu);
    public static readonly Lazy<AreaLayout> AreaLayoutIppLeftMenu = GetLazyAreaLayout(Tests06Areas.AreaIpp.Value.Id, AreaLayout.LayoutTypes.LeftMenu);
    public static readonly Lazy<AreaLayout> AreaLayoutIppRightMenu = GetLazyAreaLayout(Tests06Areas.AreaIpp.Value.Id, AreaLayout.LayoutTypes.RightMenu);
    public static readonly Lazy<AreaLayout> AreaLayoutBuhs = GetLazyAreaLayout(Tests06Areas.AreaBuhs.Value.Id, AreaLayout.LayoutTypes.NoMenu);
    public static readonly Lazy<AreaLayout> AreaLayoutBuhsLeftMenu = GetLazyAreaLayout(Tests06Areas.AreaBuhs.Value.Id, AreaLayout.LayoutTypes.LeftMenu);
    public static readonly Lazy<AreaLayout> AreaLayoutBuhsRightMenu = GetLazyAreaLayout(Tests06Areas.AreaBuhs.Value.Id, AreaLayout.LayoutTypes.RightMenu);
    public static readonly Lazy<AreaLayout> AreaLayoutNima = GetLazyAreaLayout(Tests06Areas.AreaNima.Value.Id, AreaLayout.LayoutTypes.NoMenu);
    public static readonly Lazy<AreaLayout> AreaLayoutNimaLeftMenu = GetLazyAreaLayout(Tests06Areas.AreaNima.Value.Id, AreaLayout.LayoutTypes.LeftMenu);
    public static readonly Lazy<AreaLayout> AreaLayoutNimaRightMenu = GetLazyAreaLayout(Tests06Areas.AreaNima.Value.Id, AreaLayout.LayoutTypes.RightMenu);

    [ClassInitialize]
    public static void CLassInit(TestContext context)
    {
        using IAdminContext adminContext = TestHelper.AdminContextProvider.Create();
        void AddAreaLayout(Area area, AreaLayout.LayoutTypes layoutType)
        {
            AreaLayout areaLayout = adminContext.AreaLayouts.Add(new AreaLayout
            {
                AreaId = area.AreaId,
                LayoutType = layoutType,
                LayoutName = layoutType switch
                {
                    AreaLayout.LayoutTypes.NoMenu => $"{area.AreaName.ToNullIfEmpty() ?? "buho"} Layout (No Menu)",
                    AreaLayout.LayoutTypes.LeftMenu => $"{area.AreaName.ToNullIfEmpty() ?? "buho"} Layout (Left Menu)",
                    AreaLayout.LayoutTypes.RightMenu => $"{area.AreaName.ToNullIfEmpty() ?? "buho"} Layout (Right Menu)",
                    _ => throw new ArgumentOutOfRangeException(nameof(layoutType), layoutType, null),
                },
                LayoutPath = layoutType switch
                {
                    AreaLayout.LayoutTypes.NoMenu => "~/Views/Shared/_BuLayout.cshtml",
                    AreaLayout.LayoutTypes.LeftMenu => "~/Views/Shared/_BuLeftMenuLayout.cshtml",
                    AreaLayout.LayoutTypes.RightMenu => "~/Views/Shared/_BuRightMenuLayout.cshtml",
                    _ => throw new ArgumentOutOfRangeException(nameof(layoutType), layoutType, null),
                },
                Status = AreaLayout.Statuses.Active,
            }).Entity;
            areaLayout.TrackedEntity.SetCreated(TestHelper.SuperAdministratorUserNow, TestHelper.SuperAdministratorUserId);
            areaLayout.TrackedEntity.SetLastUpdated(TestHelper.SuperAdministratorUserNow, TestHelper.SuperAdministratorUserId);
            areaLayout.PreSave(TestHelper.SuperAdministratorClaimPrincipal);
        }

        AddAreaLayout(Tests06Areas.AreaBuho.Value, AreaLayout.LayoutTypes.NoMenu);
        AddAreaLayout(Tests06Areas.AreaBuho.Value, AreaLayout.LayoutTypes.LeftMenu);
        AddAreaLayout(Tests06Areas.AreaBuho.Value, AreaLayout.LayoutTypes.RightMenu);
        AddAreaLayout(Tests06Areas.AreaBuic.Value, AreaLayout.LayoutTypes.NoMenu);
        AddAreaLayout(Tests06Areas.AreaBuic.Value, AreaLayout.LayoutTypes.LeftMenu);
        AddAreaLayout(Tests06Areas.AreaBuic.Value, AreaLayout.LayoutTypes.RightMenu);
        AddAreaLayout(Tests06Areas.AreaBukc.Value, AreaLayout.LayoutTypes.NoMenu);
        AddAreaLayout(Tests06Areas.AreaBukc.Value, AreaLayout.LayoutTypes.LeftMenu);
        AddAreaLayout(Tests06Areas.AreaBukc.Value, AreaLayout.LayoutTypes.RightMenu);
        AddAreaLayout(Tests06Areas.AreaBulc.Value, AreaLayout.LayoutTypes.NoMenu);
        AddAreaLayout(Tests06Areas.AreaBulc.Value, AreaLayout.LayoutTypes.LeftMenu);
        AddAreaLayout(Tests06Areas.AreaBulc.Value, AreaLayout.LayoutTypes.RightMenu);
        AddAreaLayout(Tests06Areas.AreaIpp.Value, AreaLayout.LayoutTypes.NoMenu);
        AddAreaLayout(Tests06Areas.AreaIpp.Value, AreaLayout.LayoutTypes.LeftMenu);
        AddAreaLayout(Tests06Areas.AreaIpp.Value, AreaLayout.LayoutTypes.RightMenu);
        AddAreaLayout(Tests06Areas.AreaBuhs.Value, AreaLayout.LayoutTypes.NoMenu);
        AddAreaLayout(Tests06Areas.AreaBuhs.Value, AreaLayout.LayoutTypes.LeftMenu);
        AddAreaLayout(Tests06Areas.AreaBuhs.Value, AreaLayout.LayoutTypes.RightMenu);
        AddAreaLayout(Tests06Areas.AreaNima.Value, AreaLayout.LayoutTypes.NoMenu);
        AddAreaLayout(Tests06Areas.AreaNima.Value, AreaLayout.LayoutTypes.LeftMenu);
        AddAreaLayout(Tests06Areas.AreaNima.Value, AreaLayout.LayoutTypes.RightMenu);
        adminContext.SaveChanges(TestHelper.SuperAdministratorClaimPrincipal);
    }

    [TestMethod]
    public void AreaDataCheck()
    {
        Assert.IsNotNull(AreaLayoutBuicRightMenu.Value);
    }
}