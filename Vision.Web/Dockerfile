FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /src
COPY ["Vision.Web/Vision.Web.csproj", "Vision.Web/"]
RUN dotnet restore "Vision.Web/Vision.Web.csproj"
COPY . .
WORKDIR "/src/Vision.Web"
RUN dotnet build "Vision.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Vision.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .

RUN chmod +x ./entrypoint.sh
CMD /bin/bash ./entrypoint.sh

# ENTRYPOINT ["dotnet", "Vision.Web.dll"]