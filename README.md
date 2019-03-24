# Vision

This project runs on .NET Core 3 Preview

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

* implement /sources/vcs/edit/{vcsId}
* implement /sources/cicds/edit/{vcsId}
* implement /sources/registry/edit/{vcsId}

* implement /tasks/
* implement /tasks/update/assets/{aId}
* implement /tasks/update/dependencies/{dId}
* implement /tasks/update/frameworks/{fId}
* implement /tasks/update/repositories/{id}

* implement /insights/assets/{dependencyKind}  
* implement /insights/dependencies/{dependencyKind}  
* implement /insights/registries/  
* implement /insights/repositories/  
* implement /insights/frameworks/  
* implement /insights/versions/{vId}  

* implement when refreshing an entire version control source that any ignored repositories are still persisted and not lost
* implement ignored repositories have all their assets removed from the system

* Add bar metric for 'On latest (%)'

* reintroduce API controllers for most of the sources

* Add gulp tasks for compiling the SASS (currently using vs code sass compiler plugin)
* Integration with SonarQube API?
* Integration tests should use Docker for API integration with: TeamCity, Jenkins & Bitbucket
* Rename CICD's to Pipelines (Teamcity pipelines, Azure Pipelines, Jenkins Pipelines, etc etc)