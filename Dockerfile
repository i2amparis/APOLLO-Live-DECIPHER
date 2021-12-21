#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["src/Topsis.Web/Topsis.Web.csproj", "src/Topsis.Web/"]
COPY ["src/Topsis.Adapters.Algorithm/Topsis.Adapters.Algorithm.csproj", "src/Topsis.Adapters.Algorithm/"]
COPY ["src/Topsis.Application/Topsis.Application.csproj", "src/Topsis.Application/"]
COPY ["src/Topsis.Domain/Topsis.Domain.csproj", "src/Topsis.Domain/"]
COPY ["src/Topsis.Adapters.Database/Topsis.Adapters.Database.csproj", "src/Topsis.Adapters.Database/"]
COPY ["src/Topsis.Adapters.Caching/Topsis.Adapters.Caching.csproj", "src/Topsis.Adapters.Caching/"]
COPY ["src/Topsis.Adapters.Email/Topsis.Adapters.Email.csproj", "src/Topsis.Adapters.Email/"]
RUN dotnet restore "src/Topsis.Web/Topsis.Web.csproj"
COPY . .
WORKDIR "/src/src/Topsis.Web"
RUN dotnet build "Topsis.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Topsis.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Topsis.Web.dll"]