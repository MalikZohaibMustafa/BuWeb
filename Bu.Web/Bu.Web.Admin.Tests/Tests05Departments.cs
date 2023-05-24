using Bu.Web.Data.Abstraction;
using Bu.Web.Data.Entities;

namespace Bu.Web.Admin.Tests;

[TestClass]
public sealed class Tests05Departments
{
    private static Lazy<Department> GetLazyDepartment(int instiuteId, string departmentAlias)
    {
        return new Lazy<Department>(() =>
        {
            using IAdminContext adminContext = TestHelper.AdminContextProvider.Create();
            return adminContext.Departments.Single(x => x.InstituteId == instiuteId && x.DepartmentAlias == departmentAlias);
        });
    }

    public static readonly Lazy<Department> DepartmentItBuho = GetLazyDepartment(Tests03Institutes.InstituteBuho.Value.Id, "IT");
    public static readonly Lazy<Department> DepartmentCsBuic = GetLazyDepartment(Tests03Institutes.InstituteBuic.Value.Id, "CS");
    public static readonly Lazy<Department> DepartmentSeBuic = GetLazyDepartment(Tests03Institutes.InstituteBuic.Value.Id, "SE");
    public static readonly Lazy<Department> DepartmentEesBuic = GetLazyDepartment(Tests03Institutes.InstituteBuic.Value.Id, "EES");
    public static readonly Lazy<Department> DepartmentCsBukc = GetLazyDepartment(Tests03Institutes.InstituteBukc.Value.Id, "CS");
    public static readonly Lazy<Department> DepartmentSeBukc = GetLazyDepartment(Tests03Institutes.InstituteBukc.Value.Id, "SE");
    public static readonly Lazy<Department> DepartmentEesBukc = GetLazyDepartment(Tests03Institutes.InstituteBukc.Value.Id, "EES");

    [ClassInitialize]
    public static void CLassInit(TestContext context)
    {
        using IAdminContext adminContext = TestHelper.AdminContextProvider.Create();
        void AddDepartment(int instituteId, int? instituteLocationId, string name, string shortName, string alias)
        {
            Department department = adminContext.Departments.Add(new Department
            {
                InstituteId = instituteId,
                InstituteLocationId = instituteLocationId,
                DepartmentName = name,
                DepartmentShortName = shortName,
                DepartmentAlias = alias,
                Status = Department.Statuses.Active,
            }).Entity;
            department.TrackedEntity.SetCreated(TestHelper.SuperAdministratorUserNow, TestHelper.SuperAdministratorUserId);
            department.TrackedEntity.SetLastUpdated(TestHelper.SuperAdministratorUserNow, TestHelper.SuperAdministratorUserId);
            department.PreSave(TestHelper.SuperAdministratorClaimPrincipal);
        }

        AddDepartment(Tests03Institutes.InstituteBuho.Value.Id, null, "IT Directorate", "IT Dte.", "IT");
        AddDepartment(Tests03Institutes.InstituteBuic.Value.Id, Tests04InstituteLocations.InstituteLocationE8Buic.Value.Id, "Computer Science", "Computer Science", "CS");
        AddDepartment(Tests03Institutes.InstituteBuic.Value.Id, Tests04InstituteLocations.InstituteLocationH11Buic.Value.Id, "Software Engineering", "Software Engg.", "SE");
        AddDepartment(Tests03Institutes.InstituteBuic.Value.Id, Tests04InstituteLocations.InstituteLocationH11Buic.Value.Id, "Earth and Environment Sciences", "Earth & Environment Sciences", "EES");
        AddDepartment(Tests03Institutes.InstituteBukc.Value.Id, null, "Computer Science", "Computer Science", "CS");
        AddDepartment(Tests03Institutes.InstituteBukc.Value.Id, null, "Software Engineering", "Software Engg.", "SE");
        AddDepartment(Tests03Institutes.InstituteBukc.Value.Id, null, "Earth and Environment Sciences", "Earth & Environment Sciences", "EES");
        adminContext.SaveChanges(TestHelper.SuperAdministratorClaimPrincipal);
    }

    [TestMethod]
    public void DepartmentDataCheck()
    {
        Assert.IsNotNull(DepartmentCsBukc.Value);
    }
}