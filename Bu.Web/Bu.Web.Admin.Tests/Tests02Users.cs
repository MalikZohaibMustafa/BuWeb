using Bu.Web.Data.Abstraction;
using Bu.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bu.Web.Admin.Tests;

[TestClass]
public sealed class Tests02Users
{
    [ClassInitialize]
    public static void CLassInit(TestContext context)
    {
        CreateSuperAdministratorUser(
             userId: TestHelper.SuperAdministratorUserId,
             email: "01-134192-030@student.bahria.edu.pk",
             name: "Malik Zohaib Mustafa",
             mobileNo: "+923341056209",
             phoneNo: "",
             departmentId: TestHelper.ItDirectorateDepartmentId);
    }

    private static void CreateSuperAdministratorUser(
        int userId,
        string email,
        string name,
        string mobileNo,
        string phoneNo,
        int departmentId)
    {
        using IAdminContext adminContext = TestHelper.AdminContextProvider.Create();
        adminContext.Database.OpenConnection();

        void SetIdentityInsert(string tableName, bool on, string schema = "dbo")
        {
            adminContext.Database.ExecuteSqlRaw($"SET IDENTITY_INSERT [{schema}].[{tableName}] {(on ? "ON" : "OFF")};");
        }

        SetIdentityInsert(nameof(adminContext.Users), true);

        User userSuperAdministrator = adminContext.Users.Add(new User
        {
            UserId = userId,
            Email = email,
            Name = name,
            MobileNo = mobileNo,
            PhoneNo = phoneNo,
            DepartmentId = departmentId,
            Status = User.Statuses.Active,
            UserRoles = new List<UserRole>
            {
                new UserRole
                {
                    Role = UserRole.Roles.SuperAdministrator,
                    Status = UserRole.Statuses.Active,
                },
                new UserRole
                {
                    Role = UserRole.Roles.Administrator,
                    Status = UserRole.Statuses.Active,
                },
                new UserRole
                {
                    Role = UserRole.Roles.Webmaster,
                    Status = UserRole.Statuses.Active,
                },
            },
        }).Entity;
        userSuperAdministrator.TrackedEntity.SetCreated(TestHelper.SuperAdministratorUserNow, TestHelper.SuperAdministratorUserId);
        userSuperAdministrator.TrackedEntity.SetLastUpdated(TestHelper.SuperAdministratorUserNow, TestHelper.SuperAdministratorUserId);
        userSuperAdministrator.ExpiringEntity.SetNoExpiry();
        userSuperAdministrator.UserRoles!.ForEach(r =>
        {
            r.TrackedEntity.SetCreated(TestHelper.SuperAdministratorUserNow, TestHelper.SuperAdministratorUserId);
            r.TrackedEntity.SetLastUpdated(TestHelper.SuperAdministratorUserNow, TestHelper.SuperAdministratorUserId);
            r.ExpiringEntity.SetNoExpiry();
        });
        userSuperAdministrator.PreSave(TestHelper.SuperAdministratorClaimPrincipal);
        adminContext.SaveChanges(TestHelper.SuperAdministratorClaimPrincipal);
        SetIdentityInsert(nameof(adminContext.Users), false);
        Assert.AreEqual(userId, userSuperAdministrator.Id);
    }

    [DataTestMethod]
    [DataRow(TestHelper.SuperAdministratorUserId, "01-134192-030@student.bahria.edu.pk")]
    public void UserDataCheck(int userId, string email)
    {
        using IAdminContext adminContext = TestHelper.AdminContextProvider.Create();
        Assert.AreEqual(1, adminContext.Users.Count(x => x.UserId == userId && x.Email == email));
    }

    [DataTestMethod]
    //[DataRow(2, "01-131192-030@student.bahria.edu.pk", "Malik Zohaib Mustafa", "+923341056209", "+923341056209", TestHelper.ItDirectorateDepartmentId)]
    [DataRow(3, "01-134202-117@student.bahria.edu.pk", "Muhammad Naeem Tahir", "+923184133228", "+923184133228", TestHelper.ItDirectorateDepartmentId)]
    public void CreateUser(
        int userId,
        string email,
        string name,
        string mobileNo,
        string phoneNo,
        int departmentId)
    {
        CreateSuperAdministratorUser(
            userId: userId,
            email: email,
            name: name,
            mobileNo: mobileNo,
            phoneNo: phoneNo,
            departmentId: departmentId);
    }
}