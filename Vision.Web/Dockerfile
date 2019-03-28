FROM # FIND PREVIEW3 RUNTIME IMAGE
WORKDIR /app
EXPOSE 80

FROM # FIND PREVIEW3 SDK IMAGE
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
ENTRYPOINT ["dotnet", "Vision.Web.dll"]
