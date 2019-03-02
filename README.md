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

* Refactor all links so that assets via navigation always go via /data/vcs/{VersionControlId}/repositories/{RepositoryId}/assets/{AssetId}
* Remove /data/assets/ as a way of navigation and linking
* Add an edit feature for each source entity to enable/disable/modify that entity  

### Icon SVG and Icon Component for Kinds

TeamCity **TODO** 
AppVeyor #00B3E0 **TODO**  
Angular #DD0031 **TODO**  
.NET #5C2D91  **TODO**  
Vue.js #4FC08D  **TODO**  

## API - Insights

Insights or Metrics? 
Controllers will get pretty chunky if they take ownership of the entity + all logic around them, split into controllers of features?


/insights/assets/{dependencyKind}  
/insights/dependencies/{dependencyKind}  
/insights/registries/  
/insights/repositories/  
/insights/frameworks/  
/insights/versions/{vId}  

## API - Tasks

/tasks/ ***TODO**  
/tasks/update/assets/{id} ***TODO**  
/tasks/update/dependencies/{id} ***TODO**  
/tasks/update/frameworks/{id} ***TODO**  
/tasks/update/repositories/{id} ***TODO**  

