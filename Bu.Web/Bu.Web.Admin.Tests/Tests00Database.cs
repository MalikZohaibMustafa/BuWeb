using Bu.AspNetCore.Core.Services;
using Bu.Web.Data;
using Bu.Web.Data.Abstraction;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Bu.Web.Admin.Tests;

[TestClass]
public sealed class Tests00Database
{
    [AssemblyInitialize]
    public static void AssemblyInit(TestContext testContext)
    {
        DateTimeHelper.InitClock(new ApplicationClock
        {
            ApplicationTimeZone = TimeZoneInfo.Local,
            LocalTimeZone = TimeZoneInfo.Local,
            GetUtcNow = () => DateTime.UtcNow,
        });
        ResetDatabase();
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        IAdminContext adminContext = TestHelper.AdminContextProvider.Create();
        adminContext.Database.ExecuteSqlRaw("ALTER TABLE [dbo].[Users] ADD CONSTRAINT [FK_Users_Departments_DepartmentId] FOREIGN KEY([DepartmentId]) REFERENCES [dbo].[Departments] ([DepartmentId]);");
        Console.WriteLine(@$"Constraint added again. [FK_Users_Departments_DepartmentId].");
    }

    private static void ResetDatabase()
    {
        SqlConnectionStringBuilder connectionStringBuilder = new SqlConnectionStringBuilder(TestHelper.BuWebConnectionString);
        string databaseName = connectionStringBuilder.InitialCatalog;
        connectionStringBuilder.InitialCatalog = "master";
        using (SqlConnection sqlConnection = new(connectionStringBuilder.ConnectionString))
        {
            sqlConnection.Open();
            using SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = $@"
        IF EXISTS (SELECT 1 FROM master.dbo.sysdatabases WHERE name = N'{databaseName}')
        BEGIN
            ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            DROP DATABASE [{databaseName}];
        END
        CREATE DATABASE [{databaseName}];";
            sqlCommand.ExecuteNonQuery();
            Console.WriteLine(@$"Dropping Database {databaseName}.");
        }

        connectionStringBuilder.InitialCatalog = databaseName;

        using IAdminContext adminContext = new AdminContextProvider(connectionStringBuilder.ConnectionString).Create();
        string script = adminContext.Database.GenerateCreateScript()
            .Replace("IF SCHEMA_ID(N'logs') IS NULL EXEC(N'CREATE SCHEMA [logs];');", "CREATE SCHEMA [logs];");

        DirectoryInfo directory = new DirectoryInfo("./");
        while (directory.Name != "Bu.Web")
        {
            directory = directory.Parent ?? throw new InvalidOperationException();
        }

        string fileName = Path.Combine(directory.FullName, "Database", "Script.sql");
        File.WriteAllText(fileName, script);
        adminContext.Database.EnsureCreated();
        adminContext.Database.ExecuteSqlRaw("ALTER TABLE [dbo].[Users] DROP CONSTRAINT [FK_Users_Departments_DepartmentId];");
        Console.WriteLine(@$"Constraint dropped. [FK_Users_Departments_DepartmentId].");
        Console.WriteLine(@$"Database {databaseName} is recreated.");
        Assert.IsTrue(adminContext.Database.CanConnect());
    }

    [TestMethod]
    public void TestDatabaseConnection()
    {
        IAdminContext adminContext = TestHelper.AdminContextProvider.Create();
        List<Data.Entities.User> users = adminContext.Users.ToList();
        Assert.IsFalse(users.Any());
    }
}