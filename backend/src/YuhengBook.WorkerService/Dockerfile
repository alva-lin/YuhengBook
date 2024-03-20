FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.*.props", "."]
COPY ["src/YuhengBook.Infrastructure/YuhengBook.Infrastructure.csproj", "src/YuhengBook.Infrastructure/"]
COPY ["src/YuhengBook.Core/YuhengBook.Core.csproj", "src/YuhengBook.Core/"]
COPY ["src/YuhengBook.UseCases/YuhengBook.UseCases.csproj", "src/YuhengBook.UseCases/"]

COPY ["src/YuhengBook.Service/YuhengBook.Service.csproj", "src/YuhengBook.Service/"]
RUN dotnet restore "src/YuhengBook.Service/YuhengBook.Service.csproj"
COPY . .

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/src/YuhengBook.Service"
RUN dotnet publish "YuhengBook.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YuhengBook.Service.dll"]
