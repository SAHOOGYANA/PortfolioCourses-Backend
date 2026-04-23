FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Folder ka naam hata diya kyunki ab hum folder ke andar hain
COPY ["PortfolioCourses.Api.csproj", "./"]
RUN dotnet restore "PortfolioCourses.Api.csproj"
COPY . .
RUN dotnet build "PortfolioCourses.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PortfolioCourses.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "PortfolioCourses.Api.dll"]