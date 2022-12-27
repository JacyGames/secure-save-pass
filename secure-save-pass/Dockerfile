#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["secure-save-pass/secure-save-pass.csproj", "secure-save-pass/"]
RUN dotnet restore "secure-save-pass/secure-save-pass.csproj"
COPY . .
WORKDIR "/src/secure-save-pass"
RUN dotnet build "secure-save-pass.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "secure-save-pass.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "secure-save-pass.dll"]