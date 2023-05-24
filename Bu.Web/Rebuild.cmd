SET BahriaNugets=https://pkgs.dev.azure.com/bahria/_packaging/BahriaNugets/nuget/v3/index.json
SET DataProject=Bu.Web.Data/Bu.Web.Data.csproj
SET WebsiteProject=Bu.Web.Website/Bu.Web.Website.csproj
SET AdminProject=Bu.Web.Admin/Bu.Web.Admin.csproj
SET CoreNuget=Bu.AspNetCore.Core
SET BootstrapHelpersNuget=Bu.AspNetCore.BootstrapHelpers
SET MvcNuget=Bu.AspNetCore.Mvc
SET Verbosity=Quiet

dotnet nuget update source BahriaNugets --source "%BahriaNugets%" -u "%BuNugetUsername%"  -p "%BuNugetPAT%"
dotnet remove "%DataProject%" package "%CoreNuget%"
dotnet add %DataProject% package %CoreNuget% -s %BahriaNugets%
dotnet clean %DataProject% --verbosity %Verbosity%
dotnet restore --force --no-cache --verbosity %Verbosity%
dotnet build %DataProject% --verbosity %Verbosity%

dotnet clean %WebsiteProject% --verbosity %Verbosity%
dotnet remove %WebsiteProject% package %BootstrapHelpersNuget%
dotnet remove %WebsiteProject% package %MvcNuget%
dotnet add %WebsiteProject% package %BootstrapHelpersNuget% -s %BahriaNugets%
dotnet add %WebsiteProject% package %MvcNuget% -s %BahriaNugets%
dotnet build %WebsiteProject% --verbosity %Verbosity%

dotnet clean %AdminProject% --verbosity %Verbosity%
dotnet remove %AdminProject% package %BootstrapHelpersNuget%
dotnet remove %AdminProject% package %MvcNuget%
dotnet add %AdminProject% package %BootstrapHelpersNuget% -s %BahriaNugets%
dotnet add %AdminProject% package %MvcNuget% -s %BahriaNugets%
dotnet build %AdminProject% --verbosity %Verbosity%

pause