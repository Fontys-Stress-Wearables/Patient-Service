FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Patient-Service/Patient-Service.csproj", "Patient-Service/"]
RUN dotnet restore "Patient-Service/Patient-Service.csproj"
COPY . .
WORKDIR "/src/Patient-Service"
RUN dotnet build "Patient-Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Patient-Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Patient-Service.dll"]
