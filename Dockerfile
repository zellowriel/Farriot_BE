# Sử dụng runtime .NET 10 cho môi trường chạy
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# Sử dụng SDK .NET 10 để build code
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy các file csproj để restore thư viện trước
COPY ["EXE_Farriott_BE.API/EXE_Farriott_BE.API.csproj", "EXE_Farriott_BE.API/"]
COPY ["EXE_Farriott_BE.Services/EXE_Farriott_BE.Services.csproj", "EXE_Farriott_BE.Services/"]
COPY ["EXE_Farriott_BE.Repositories/EXE_Farriott_BE.Repositories.csproj", "EXE_Farriott_BE.Repositories/"]
RUN dotnet restore "EXE_Farriott_BE.API/EXE_Farriott_BE.API.csproj"

# Copy toàn bộ mã nguồn của cả 3 project
COPY . .
WORKDIR "/src/EXE_Farriott_BE.API"
RUN dotnet build "EXE_Farriott_BE.API.csproj" -c Release -o /app/build

# Publish ứng dụng
FROM build AS publish
RUN dotnet publish "EXE_Farriott_BE.API.csproj" -c Release -o /app/publish

# Copy bản publish sang môi trường chạy
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EXE_Farriott_BE.API.dll"]