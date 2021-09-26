#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["File-Comparison/File-Comparison.csproj", "File-Comparison/"]
RUN dotnet restore "File-Comparison/File-Comparison.csproj"
COPY . .
WORKDIR "/src/File-Comparison"
RUN dotnet build "File-Comparison.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "File-Comparison.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "File-Comparison.dll"]