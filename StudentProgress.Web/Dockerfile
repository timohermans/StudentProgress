#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
COPY ["StudentProgress.Web/StudentProgress.Web.csproj", "StudentProgress.Web/"]
RUN dotnet restore "StudentProgress.Web/StudentProgress.Web.csproj"
COPY . .
WORKDIR "/src/StudentProgress.Web"
RUN dotnet build "StudentProgress.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "StudentProgress.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StudentProgress.Web.dll"]