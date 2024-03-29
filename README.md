# Vision

This project runs on .NET Core 3 Preview

## About

Vision is a POC platform that allows customers to integrate their chosen technology stack and ecosystems in order to provide useful insights about the maturity, security and agility of their technology health.

### Graphs and Statistics cover

- dependencies and versions of those dependencies
- language ratio across tech estate similar to Github code file statistics
- pipeline build times
- docker images detected and versions
- most and least used dependencies across all projects
- reverse lookups in order to navigate to source code directly from builds and artifacts
- the number of assets and artifacts in registries that are and arent being used

### What does this achieve?

- This platform aims to drive technical teams forward by highlighting areas of the tech estate that just are not inline with other teams in the company or are falling behind, to help make strategic decisions about migrating estates or products to different languages, or platforms
- Provide engineering managers, principals, architects to get a large overview spanning a companies stack after company aqcuistions, moving across teams etc
- Asset health reports are common practice, this also helps produce reports from actual sources, rather than manually managed spreadsheets by the squads or managers of teams
- Help drive agility within the company and to enable teams to take action by making these issues visible at a higher level and in an easier way to ingest data to understand their tech estate

## Technologies / Frameworks

* [.NET Core 3.0](https://devblogs.microsoft.com/dotnet/announcing-net-core-3-preview-1-and-open-sourcing-windows-desktop-frameworks/)
* [NSwag](https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-nswag?view=aspnetcore-2.2&tabs=visual-studio%2Cvisual-studio-xml)
* [.NET Core Web API](https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/conventions?view=aspnetcore-2.2)
* [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
* [Razor components](https://docs.microsoft.com/en-us/aspnet/core/razor-components/components?view=aspnetcore-3.0) (SPA-like behaviour with C#/Razor)
* [Libman](https://docs.microsoft.com/en-us/aspnet/core/client-side/libman/) (front end library manager by microsoft)
* [Bootstrap 4](https://getbootstrap.com/) (scss + custom with bootstrap admin 2 theme)
* [Bootstrap admin 2 theme](https://startbootstrap.com/themes/sb-admin-2/) (open source theme based on bootstrap 4)
* [Font awesome 5](https://fontawesome.com/)
* Various js libraries (jQuery, datatables, chart.js)

## Github issues
https://github.com/aspnet/AspNetCore/issues/5562  
 
## TODO

* implement /sources/vcs/edit/{id}
* implement /sources/cicds/edit/{id}
* implement /sources/registry/edit/{id}

* implement /tasks/
* implement /tasks/update/assets/{id}
* implement /tasks/update/dependencies/{id}
* implement /tasks/update/frameworks/{id}
* implement /tasks/update/repositories/{id}

* implement /insights/assets/
* implement /insights/dependencies/
* implement /insights/registries/  
* implement /insights/repositories/  
* implement /insights/frameworks/  
* implement /insights/versions/

* implement when refreshing an entire version control source that any ignored repositories are still persisted and not lost
* implement ignored repositories have all their assets removed from the system
* Add metric for 'Version asset (%)'

* reintroduce API controllers for most of the sources (ODdata for .NET Core is out - NSwag/Swashbuckle integration)

* Add gulp tasks for compiling the SASS (currently using vs code sass compiler plugin)
* Integration with SonarQube API?
* Integration tests should use Docker for API integration with: TeamCity, Jenkins & Bitbucket
* Rename CICD's to Pipelines (Teamcity pipelines, Azure Pipelines, Jenkins Pipelines, etc etc)

## Paid gateways

* Limit the number of displayed metrics
* Limit the number of repositories that will be scanned
* Limit the number of added VCS servers allowed
* Limit the number of refresh tasks allowed

* Only display the number of different dependencies found, not the individual dependency versions used
* Only provide support for a limited number of asset kinds. (i.e .NET, Maven, NPM)

* Disable CICD reverse lookup integration
* Disable information on repositories that publish dependencies
* Disable information on external assets that depend on a repository (similar to Github Dependeny graph - Dependents tab)
* Disable information on the latest version of a dependency
* Disable export or reporting functionality
* Disable integration with Security tools such as SonarQube or other scanners which provide APIs

