using Bu.Web.Data.Abstraction;
using Bu.Web.Data.Entities;

namespace Bu.Web.Admin.Tests;

[TestClass]
public sealed class Tests01IpAddresses
{
    [ClassInitialize]
    public static void CLassInit(TestContext context)
    {
        using IAdminContext adminContext = TestHelper.AdminContextProvider.Create();
        adminContext.WhitelistedIpAddresses.Add(new WhitelistedIpAddress
        {
            IpAddress = System.Net.IPAddress.Loopback.ToString(),
        });
        adminContext.WhitelistedIpAddresses.Add(new WhitelistedIpAddress
        {
            IpAddress = System.Net.IPAddress.IPv6Loopback.ToString(),
        });
        adminContext.SaveChanges(TestHelper.SuperAdministratorClaimPrincipal);
    }

    [DataTestMethod]
    [DataRow("127.0.0.1")]
    [DataRow("::1")]
    public void IpAddressDataCheck(string ipAddress)
    {
        using IAdminContext adminContext = TestHelper.AdminContextProvider.Create();
        Assert.AreEqual(1, adminContext.WhitelistedIpAddresses.Count(x => x.IpAddress == ipAddress));
    }
}
