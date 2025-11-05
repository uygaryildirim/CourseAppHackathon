## 📋 Paket Listesi

### CourseApp.API (Ana Proje)

**Dosya:** `CourseApp.API/CourseApp.API.csproj`

#### Web Framework
- `Microsoft.AspNetCore.OpenApi` (9.0.0) - OpenAPI/Swagger desteği
- `Swashbuckle.AspNetCore` (6.6.2) - Swagger UI

#### Database
- `Microsoft.EntityFrameworkCore.InMemory` (9.0.0) - InMemory Database

#### Mapping
- `AutoMapper` (12.0.0) - Entity-DTO mapping
- `AutoMapper.Extensions.Microsoft.DependencyInjection` (12.0.0) - AutoMapper DI extension

#### Validation
- `FluentValidation.AspNetCore` (11.3.0) - Input validation
- `FluentValidation.DependencyInjectionExtensions` (11.9.0) - FluentValidation DI extension

#### Logging
- `Serilog.AspNetCore` (8.0.0) - Structured logging
- `Serilog.Sinks.Console` (5.0.1) - Console logging
- `Serilog.Sinks.File` (5.0.0) - File logging

#### Health Checks
- `Microsoft.Extensions.Diagnostics.HealthChecks` (9.0.0) - Health check infrastructure
- `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore` (9.0.0) - EF Core health checks

#### Rate Limiting
- `AspNetCoreRateLimit` (5.0.0) - API rate limiting

---

### CourseApp.DataAccessLayer

**Dosya:** `CourseApp.DataAccessLayer/CourseApp.DataAccessLayer.csproj`

#### Entity Framework Core
- `Microsoft.EntityFrameworkCore.Design` (9.0.0) - EF Core design-time tools
- `Microsoft.EntityFrameworkCore.SqlServer` (9.0.0) - SQL Server provider (şu anda kullanılmıyor, InMemory kullanılıyor)
- `Microsoft.EntityFrameworkCore.Tools` (9.0.0) - EF Core tools (migrations, vb.)

---

### CourseApp.ServiceLayer

**Dosya:** `CourseApp.ServiceLayer/CourseApp.BusinessLayer.csproj`

#### Mapping
- `AutoMapper` (12.0.0) - Entity-DTO mapping
- `AutoMapper.Extensions.Microsoft.DependencyInjection` (12.0.0) - AutoMapper DI extension

---

### CourseApp.Tests

**Dosya:** `CourseApp.Tests/CourseApp.Tests.csproj`

#### Test Framework
- `xunit` (2.9.2) - Unit test framework
- `xunit.runner.visualstudio` (2.8.2) - Visual Studio test runner
- `Microsoft.NET.Test.Sdk` (17.12.0) - Test SDK

#### Mocking
- `Moq` (4.20.72) - Mock framework
- `MockQueryable.Moq` (7.0.3) - IQueryable mocking

#### Assertions
- `FluentAssertions` (7.0.0) - Okunabilir assertion'lar

#### Code Coverage
- `coverlet.collector` (6.0.2) - Code coverage collector

---

## 🔧 Paket Yönetimi Komutları

### Tüm Paketleri Geri Yükleme

```bash
# Solution root'undan
dotnet restore

# Veya belirli bir proje için
dotnet restore CourseApp.API/CourseApp.API.csproj
```

### Paket Ekleme

```bash
# Belirli bir projeye paket ekleme
cd CourseApp.API
dotnet add package PackageName --version X.Y.Z

# Örnek: Serilog ekleme
dotnet add package Serilog.AspNetCore --version 8.0.0
```

### Paket Güncelleme

```bash
# Tüm paketleri güncelleme
dotnet list package --outdated

# Belirli bir paketi güncelleme
dotnet add package PackageName --version X.Y.Z
```

### Paket Kaldırma

```bash
# Paket kaldırma
dotnet remove package PackageName
```

### Paket Listesi Görüntüleme

```bash
# Tüm projelerdeki paketleri listele
dotnet list package

# Belirli bir projede
dotnet list package --project CourseApp.API/CourseApp.API.csproj
```

---

## 📝 .csproj Dosyası Örneği

Paketler `.csproj` dosyasında şu şekilde tanımlanır:

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net9.0</TargetFramework>
  </PropertyGroup>

  <ItemGroup>
    <!-- Paket referansları burada tanımlanır -->
    <PackageReference Include="AutoMapper" Version="12.0.0" />
    <PackageReference Include="FluentValidation.AspNetCore" Version="11.3.0" />
  </ItemGroup>

  <ItemGroup>
    <!-- Proje referansları (diğer .NET projeleri) -->
    <ProjectReference Include="..\CourseApp.ServiceLayer\CourseApp.BusinessLayer.csproj" />
  </ItemGroup>
</Project>
```

---

## 🔄 Paket Yönetimi Workflow'u

1. **Paket Ekleme:**
   ```bash
   dotnet add package PackageName --version X.Y.Z
   ```
   Bu komut otomatik olarak `.csproj` dosyasını günceller.

2. **Paket İndirme:**
   ```bash
   dotnet restore
   ```
   `.csproj` dosyasındaki tüm paketler NuGet'ten indirilir.

3. **Proje Derleme:**
   ```bash
   dotnet build
   ```
   İndirilen paketlerle proje derlenir.

---

## 📚 İlgili Dokümantasyonlar

- [.NET Package Management](https://learn.microsoft.com/en-us/nuget/)
- [NuGet Package Manager](https://www.nuget.org/)
- [.csproj File Format](https://learn.microsoft.com/en-us/dotnet/core/tools/csproj)

---

**Son Güncelleme:** 2025-11-05  
**.NET Version:** 9.0  
**Paket Yöneticisi:** NuGet (dotnet CLI)

