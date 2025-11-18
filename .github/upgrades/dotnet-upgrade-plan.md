# .NET 10.0 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that an .NET 10.0 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10.0 upgrade.
3. Upgrade Security.Sanitization\Security.Sanitization.csproj
4. Upgrade Security.Sanitization.Api\Security.Sanitization.Api.csproj
5. Upgrade Security.Sanitization.Tests.Integration\Security.Sanitization.Tests.Integration.csproj
6. Upgrade Security.Sanitization.Tests.Unit\Security.Sanitization.Tests.Unit.csproj

## Settings

This section contains settings and data used by execution steps.

### Excluded projects

Table below contains projects that do belong to the dependency graph for selected projects and should not be included in the upgrade.

| Project name                                   | Description                 |
|:-----------------------------------------------|:---------------------------:|
| None                                           | No projects explicitly excluded |

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                        | Current Version | New Version | Description                                   |
|:------------------------------------|:---------------:|:-----------:|:----------------------------------------------|
| Microsoft.AspNetCore.Mvc.Testing    |   8.0.20        |  10.0.0     | Recommended replacement for .NET 10 preview    |
| Microsoft.AspNetCore.OpenApi        |   8.0.20        |  10.0.0     | Recommended replacement for .NET 10 preview    |

### Project upgrade details
This section contains details about each project upgrade and modifications that need to be done in the project.

#### Security.Sanitization modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - No package changes detected for this project.

Feature upgrades:
  - None.

Other changes:
  - None.

#### Security.Sanitization.Api modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - `Microsoft.AspNetCore.OpenApi` should be updated from `8.0.20` to `10.0.0`.

Feature upgrades:
  - None.

Other changes:
  - None.

#### Security.Sanitization.Tests.Integration modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - `Microsoft.AspNetCore.Mvc.Testing` should be updated from `8.0.20` to `10.0.0`.

Feature upgrades:
  - None.

Other changes:
  - None.

#### Security.Sanitization.Tests.Unit modifications

Project properties changes:
  - Target framework should be changed from `net8.0` to `net10.0`

NuGet packages changes:
  - No package changes detected for this project.

Feature upgrades:
  - None.

Other changes:
  - None.
