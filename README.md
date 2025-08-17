# KayraExport Case 3 - Microservices Architecture

Bu proje, .NET 9 kullanılarak geliştirilmiş, Onion mimari ve SOLID prensipleri ile tasarlanmış mikroservis mimarisidir. Proje, modern backend geliştirme tekniklerini ve 12 Faktör Uygulama metodolojisini uygular.

## 🏗️ Mimari Yapı

### Onion Architecture (Soğan Mimarisi)
- **Core Layer**: Domain entities, interfaces, and business logic
- **Application Layer**: Use cases, DTOs, and application services
- **Infrastructure Layer**: Data access, external services, and implementations
- **Presentation Layer**: Controllers and API endpoints

### Mikroservisler
1. **Auth Service** - Kimlik doğrulama ve yetkilendirme
2. **Product Service** - Ürün yönetimi (CQRS pattern)
3. **Log Service** - Merkezi loglama
4. **API Gateway** - Yarp ile merkezi yönlendirme ve API yönetimi

## 🚀 Teknolojiler

- **.NET 9** - Ana framework
- **C#** - Programlama dili
- **Entity Framework Core** - ORM
- **SQL Server** - Veritabanı
- **Redis** - Cache
- **JWT** - Kimlik doğrulama
- **Yarp** - Reverse proxy
- **MediatR** - CQRS implementation
- **Serilog** - Structured logging
- **Docker** - Containerization

## 📋 Gereksinimler

- .NET 9 SDK
- SQL Server (LocalDB veya SQL Server Express)
- Redis (opsiyonel, cache için)
- Docker (opsiyonel)

## 🛠️ Kurulum

### 1. Projeyi Klonlayın
```bash
git clone <repository-url>
cd KayraExport.Case3
```

### 2. Bağımlılıkları Yükleyin
```bash
dotnet restore
```

### 3. Veritabanlarını Oluşturun
Her servis için ayrı veritabanı oluşturulacaktır:
- AuthServiceDb
- ProductServiceDb
- LogServiceDb

### 4. Servisleri Çalıştırın

#### Auth Service
```bash
cd AuthService
dotnet run
```
**Port:** 7001

#### Product Service
```bash
cd ProductService
dotnet run
```
**Port:** 7002

#### Log Service
```bash
cd LogService
dotnet run
```
**Port:** 7003

#### API Gateway
```bash
cd ApiGateway
dotnet run
```
**Port:** 7000

## 🐳 Docker ile Çalıştırma

### Tüm servisleri Docker ile çalıştırın:
```bash
docker-compose up -d
```

### Servisleri durdurun:
```bash
docker-compose down
```

## 📚 API Dokümantasyonu

### Auth Service Endpoints

#### POST /api/auth/register
Kullanıcı kaydı
```json
{
  "username": "testuser",
  "email": "test@example.com",
  "password": "Test123!",
  "firstName": "Test",
  "lastName": "User"
}
```

#### POST /api/auth/login
Kullanıcı girişi
```json
{
  "username": "testuser",
  "password": "Test123!"
}
```

#### POST /api/auth/refresh
Token yenileme
```json
{
  "refreshToken": "refresh-token-here"
}
```

### Product Service Endpoints

#### GET /api/product
Ürün listesi (sayfalama ve filtreleme ile)
```
GET /api/product?pageNumber=1&pageSize=10&category=Electronics
```

#### POST /api/product
Yeni ürün oluşturma (JWT gerekli)
```json
{
  "name": "Sample Product",
  "description": "Product description",
  "price": 99.99,
  "stockQuantity": 100,
  "category": "Electronics",
  "sku": "PROD-001"
}
```

#### PUT /api/product/{id}
Ürün güncelleme (JWT gerekli)
```json
{
  "name": "Updated Product Name",
  "price": 89.99
}
```

#### DELETE /api/product/{id}
Ürün silme (JWT gerekli)

### Log Service Endpoints

#### POST /api/log
Log entry oluşturma
```json
{
  "serviceName": "ProductService",
  "level": "INFO",
  "message": "Product created successfully",
  "contextData": "{\"productId\": \"123\", \"userId\": \"456\"}",
  "correlationId": "corr-123"
}
```

#### GET /api/log
Log entries listesi
```
GET /api/log?serviceName=ProductService&level=ERROR&pageNumber=1&pageSize=50
```

#### GET /api/log/statistics/{serviceName}
Servis log istatistikleri
```
GET /api/log/statistics/ProductService?fromDate=2024-01-01&toDate=2024-12-31
```

## 🔐 Kimlik Doğrulama

JWT token kullanarak kimlik doğrulama yapılır:

1. `/api/auth/login` endpoint'inden token alın
2. `Authorization: Bearer {token}` header'ı ile API çağrıları yapın

## 🗄️ Veritabanı Şeması

### Users Tablosu (AuthService)
- Id (Guid, PK)
- Username (string, unique)
- Email (string, unique)
- PasswordHash (string)
- FirstName (string)
- LastName (string)
- Roles (JSON array)
- CreatedAt (DateTime)
- UpdatedAt (DateTime?)
- IsActive (bool)

### Products Tablosu (ProductService)
- Id (Guid, PK)
- Name (string)
- Description (string)
- Price (decimal)
- StockQuantity (int)
- Category (string)
- SKU (string, unique)
- CreatedAt (DateTime)
- UpdatedAt (DateTime?)
- IsActive (bool)

### LogEntries Tablosu (LogService)
- Id (Guid, PK)
- ServiceName (string)
- Level (string)
- Message (string)
- ContextData (string)
- Exception (string)
- UserId (Guid?)
- CorrelationId (string)
- IpAddress (string)
- UserAgent (string)
- CreatedAt (DateTime)
- UpdatedAt (DateTime?)
- IsActive (bool)

## 🏗️ Design Patterns

### CQRS (Command Query Responsibility Segregation)
- **Commands**: Veri değiştirme işlemleri (Create, Update, Delete)
- **Queries**: Veri okuma işlemleri (Get, List, Search)

### Repository Pattern
Veri erişim katmanı için repository interface'leri kullanılır.

### Event-Driven Architecture
Domain event'ler ile servisler arası iletişim sağlanır.

## 📊 Cache Stratejisi

Redis kullanılarak ürün sorguları cache'lenir:
- Cache key pattern: `products:page:{page}:size:{size}:category:{category}`
- Cache expiration: 30 dakika
- Cache invalidation: Ürün ekleme/güncelleme sonrası

## 🔄 Event Bus

Şu anda placeholder implementation kullanılmaktadır. Gerçek implementasyon için:
- RabbitMQ
- Apache Kafka
- Azure Service Bus

## 📈 Monitoring ve Logging

### Structured Logging
- Serilog kullanılarak JSON formatında loglar
- Log seviyeleri: INFO, WARNING, ERROR, CRITICAL
- Correlation ID ile request tracing

### Centralized Logging
Tüm servislerin logları Log Service'te toplanır.

## 🚦 Rate Limiting

API Gateway seviyesinde rate limiting:
- 100 request per minute per user/IP
- Fixed window rate limiter

## 🔒 Güvenlik

- JWT token authentication
- HTTPS enforcement
- CORS policy
- Input validation
- SQL injection protection

## 🧪 Test

### Unit Tests
```bash
dotnet test
```

### Integration Tests
Her servis için ayrı test projeleri oluşturulacaktır.

## 📦 Deployment

### Production
```bash
dotnet publish -c Release
```

### Docker
```bash
docker build -t kayraexport-case3 .
docker run -p 7000:80 kayraexport-case3
```

## 🔧 Konfigürasyon

### Environment Variables
- `ConnectionStrings__DefaultConnection`: Veritabanı bağlantı string'i
- `ConnectionStrings__Redis`: Redis bağlantı string'i
- `Jwt__SecretKey`: JWT imzalama anahtarı
- `Jwt__Issuer`: JWT issuer
- `Jwt__Audience`: JWT audience

### appsettings.json
Her servis için ayrı konfigürasyon dosyası bulunur.

