FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/LoanApplications.Api/LoanApplications.Api.csproj", "src/LoanApplications.Api/"]
COPY ["src/LoanApplications.Application/LoanApplications.Application.csproj", "src/LoanApplications.Application/"]
COPY ["src/LoanApplications.Domain/LoanApplications.Domain.csproj", "src/LoanApplications.Domain/"]
COPY ["src/LoanApplications.Infrastructure/LoanApplications.Infrastructure.csproj", "src/LoanApplications.Infrastructure/"]
RUN dotnet restore "src/LoanApplications.Api/LoanApplications.Api.csproj"
COPY . .
WORKDIR "/src/src/LoanApplications.Api"
RUN dotnet build "./LoanApplications.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LoanApplications.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LoanApplications.Api.dll"]
