# Etapa de compilación
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copiar archivos de proyecto y restaurar dependencias
COPY ["src/BancoSanabria.API/BancoSanabria.API.csproj", "src/BancoSanabria.API/"]
COPY ["src/BancoSanabria.Application/BancoSanabria.Application.csproj", "src/BancoSanabria.Application/"]
COPY ["src/BancoSanabria.Domain/BancoSanabria.Domain.csproj", "src/BancoSanabria.Domain/"]
COPY ["src/BancoSanabria.Infrastructure/BancoSanabria.Infrastructure.csproj", "src/BancoSanabria.Infrastructure/"]

RUN dotnet restore "src/BancoSanabria.API/BancoSanabria.API.csproj"

# Copiar el resto del código y compilar
COPY src/ ./src/
WORKDIR "/src/src/BancoSanabria.API"
RUN dotnet build "BancoSanabria.API.csproj" -c Release -o /app/build

# Etapa de publicación
FROM build AS publish
RUN dotnet publish "BancoSanabria.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BancoSanabria.API.dll"]

