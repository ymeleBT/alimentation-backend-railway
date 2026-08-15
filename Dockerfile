FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY src/AlimentationDongmo.Domain/AlimentationDongmo.Domain.csproj src/AlimentationDongmo.Domain/
COPY src/AlimentationDongmo.Infrastructure/AlimentationDongmo.Infrastructure.csproj src/AlimentationDongmo.Infrastructure/
COPY src/AlimentationDongmo.Api/AlimentationDongmo.Api.csproj src/AlimentationDongmo.Api/
RUN dotnet restore src/AlimentationDongmo.Api/AlimentationDongmo.Api.csproj

COPY src/AlimentationDongmo.Domain/ src/AlimentationDongmo.Domain/
COPY src/AlimentationDongmo.Infrastructure/ src/AlimentationDongmo.Infrastructure/
COPY src/AlimentationDongmo.Api/ src/AlimentationDongmo.Api/

RUN dotnet publish src/AlimentationDongmo.Api/AlimentationDongmo.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "AlimentationDongmo.Api.dll"]
