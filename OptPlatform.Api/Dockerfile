FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["OptPlatform.Api/OptPlatform.Api.csproj", "OptPlatform.Api/"]
COPY ["OptPlatform.Application/OptPlatform.Application.csproj", "OptPlatform.Application/"]
COPY ["OptPlatform.Domain/OptPlatform.Domain.csproj", "OptPlatform.Domain/"]
COPY ["OptPlatform.Infrastructure/OptPlatform.Infrastructure.csproj", "OptPlatform.Infrastructure/"]
RUN dotnet restore "OptPlatform.Api/OptPlatform.Api.csproj"
COPY . .
WORKDIR "/src/OptPlatform.Api"
RUN dotnet build "OptPlatform.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "OptPlatform.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OptPlatform.Api.dll"]