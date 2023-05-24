using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Bu.Web.TestData")]
[assembly: InternalsVisibleTo("Bu.Web.Admin.Tests")]

new Bu.Web.Admin.Startup().Run(args);