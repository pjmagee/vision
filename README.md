# Vision

NSwag: https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-nswag?view=aspnetcore-2.2&tabs=visual-studio%2Cvisual-studio-xml    
API Conventions: https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/conventions?view=aspnetcore-2.2    
Data annotations: https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-nswag?view=aspnetcore-2.2&tabs=visual-studio%2Cvisual-studio-code-xml#data-annotations    

## Technologies / Frameworks

* .NET Core 3.0 Preview 2
* .NET Core Web API
* Entity Framework Core
* Razor components (SPA like framework)
* Libman (front end library manager by microsoft)
* Bootstrap 4 (scss + custom with bootstrap admin 2 theme)
* Bootstrap admin 2 theme (open source theme based on bootstrap 4)
* Font awesome 5
* Various js libraries (jQuery, datatables, chart.js, jquery easing)

## Github issues

https://github.com/aspnet/AspNetCore/issues/5562  
 
## Current pages

/vcs/ **DONE**  
/vcs/{id} **DONE**  
/repositories/{id}  **DONE**  
/registries/ **DONE**  
/registries/{id} **DONE**  
/cicds/ **DONE**  
/dependencies/ **DONE**  
/dependencies/{id} **DONE**  
/frameworks/ **DONE**  
/frameworks/{id} **DONE**  
/assets/ **DONE**  
/assets/{id} **DONE**  
/versions/{id} **DONE**  

## Source pages (next phase)

/sources/vcs **TODO**  
/sources/vcs/{vId} - edit **TODO**  
/sources/cicds  **TODO**  
/sources/cicds/{cId} - edit **TODO**  
/sources/registries  **TODO**  
/sources/registries/{rId} - edit **TODO**  

## Data pages (next phase)

/data/vcs/ - display all version control systems **TODO**   
/data/vcs/{id} - display [repositories] of [vcs]  **TODO**  
/data/vcs/{id}/repositories/{id}/ - display [assets, frameworks, metrics] of [repository]  **TODO**  
/data/vcs/{id}/repositories/{id}/assets/{id} - displays [dependencies, frameworks, metrics] of [asset]  **TODO**  
/data/vcs/{id}/repositories/{id}/frameworks/{id} - displays [assets, metrics] of [framework]**TODO**    
/data/dependencies/ - displays all [dependencies, metrics]  **TODO**  
/data/dependencies/{id} - displays [assets, versions, metrics] of [dependency]  **TODO**  
/data/dependencies/{id}/versions/{id}/ - displays [assets, metrics] of [version]  **TODO**  
/data/frameworks/ - displays all frameworks  **TODO**  
/data/frameworks/{id} - displays all [assets] of [framework]  **TODO**  
/data/registries - displays all dependency registries  **TODO**  
/data/registries/{id}  - displays [dependencies] of [registry]  **TODO**  
/data/cicds  **TODO**  
/data/security  **TODO**  

### Icon SVG and Icon Component for Kinds

Download all Brand/Kind/Type Icons required  
  
TeamCity **TODO**  

NuGet #004880
Jenkins #D24939
Docker #1488C6
NPM #CB3837
Gitlab #E24329
Bitbucket #0052CC
Github #181717
Java #007396
Python #3776AB
AppVeyor #00B3E0
Angular #DD0031
Ruby #CC342D
.NET #5C2D91
Vue.js #4FC08D

### Shared components

FrameworksComponent **DONE**  
AssetsComponent **DONE**  
CiCdsComponent **DONE**  
DependenciesComponent **DONE**  
MetricsComponent **DONE**  

## API - Metrics

/versioncontrols/{id}/metrics **TODO**  
/registries/{id}/metrics ***TODO**  
/repositories/{id}/metrics ***TODO**  
/dependencies/{id}/metrics ***TODO**  
/frameworks/{id}/metrics ***TODO**  
/versions/{id}/metrics ***TODO**  
/assets/{id}/metrics ***TODO**  

## API - Tasks

/tasks/ ***TODO**  
/tasks/update/assets/{id} ***TODO**  
/tasks/update/dependencies/{id} ***TODO**  
/tasks/update/frameworks/{id} ***TODO**  
/tasks/update/repositories/{id} ***TODO**  

