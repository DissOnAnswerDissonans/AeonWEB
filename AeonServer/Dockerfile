#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 2363
EXPOSE 2366

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["AeonServer/AeonServer.csproj", "AeonServer/"]
COPY ["AeonCore/AeonCore.csproj", "AeonCore/"]
COPY ["Aeon.Base/Aeon.Base.csproj", "Aeon.Base/"]
COPY ["Aeon.Heroes/Aeon.Heroes.csproj", "Aeon.Heroes/"]
RUN dotnet restore "AeonServer/AeonServer.csproj"
COPY . .
WORKDIR "/src/AeonServer"
RUN dotnet build "AeonServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AeonServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AeonServer.dll"]
