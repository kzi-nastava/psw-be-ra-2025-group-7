# Tour Execution Feature Implementation

## Pregled implementacije

Implementirana je funkcionalnost za izvršavanje tura (Tour Execution) sa sledećim karakteristikama:

### Zahtevi implementirani

1. **Pokretanje ture**
   - Turista može pokrenuti turu čime se kreira sesija (TourExecution)
   - Beleži se početna lokacija turiste (latitude/longitude)
   - Tura mora biti kupljena (TourPurchaseToken) pre pokretanja
   - Mogu se pokrenuti objavljene (Published) i arhivirane (Archived) ture

2. **Završavanje sesije**
   - Turista može kompletirati turu (Completed status)
   - Turista može napustiti turu (Abandoned status)
   - Evidentira se vreme završetka/napuštanja

3. **Otključavanje tajni**
   - Kada turista dođe do ključne tačke (KeyPoint), otključava se tajna
   - Provera udaljenosti - turista mora biti u krugu od 100m od tačke
   - Turista može pristupiti tajni tek nakon što je tačka otključana

## Arhitektura i DDD principi

### Domain Layer (Explorer.Tours.Core)

#### Entities

**TourExecution** (Aggregate Root)
```csharp
public class TourExecution : Entity
{
    public long TouristId { get; init; }
    public long TourId { get; init; }
    public DateTime StartedAt { get; init; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? AbandonedAt { get; private set; }
    public TourExecutionStatus Status { get; private set; }
    public double StartLatitude { get; init; }
    public double StartLongitude { get; init; }
    
    // Enkapsulirana lista otključanih tačaka
    private readonly List<int> _unlockedKeyPointIndices;
    public IReadOnlyList<int> UnlockedKeyPointIndices => _unlockedKeyPointIndices.AsReadOnly();
}
```

**Invarijante i biznis pravila:**
- Status može biti: Active, Completed, Abandoned
- Samo aktivne sesije mogu biti kompletirane ili napuštene
- Otključavanje tačaka je moguće samo na aktivnoj sesiji
- Koordinate moraju biti validne (lat: -90 do 90, lon: -180 do 180)

#### Repository Interface
```csharp
public interface ITourExecutionRepository
{
    TourExecution Get(long id);
    TourExecution Create(TourExecution execution);
    TourExecution Update(TourExecution execution);
    void Delete(long id);
    TourExecution? GetActiveExecutionForTourist(long touristId, long tourId);
    PagedResult<TourExecution> GetByTouristId(long touristId, int page, int pageSize);
    List<TourExecution> GetAllActiveExecutionsByTourist(long touristId);
}
```

### Application Layer (Explorer.Tours.Core.UseCases)

**TourExecutionService** - implementira biznis logiku:
- Validacija prekondita (kupljena tura, validni statusi)
- Provera aktivnih sesija
- Računanje distance između turiste i ključne tačke (Haversine formula)
- Kontrola pristupa tajnama KeyPoint-ova

### Infrastructure Layer (Explorer.Tours.Infrastructure)

**TourExecutionDbRepository** - implementacija perzistencije:
- EF Core mapiranje sa Include-ovima za Tour i KeyPoints
- JSONB kolona za skladištenje _unlockedKeyPointIndices liste (PostgreSQL)
- Optimizovani indeksi za brze pretrage

**DbContext konfiguracija:**
```csharp
modelBuilder.Entity<TourExecution>(b =>
{
    b.ToTable("TourExecutions");
    b.Property<List<int>>("_unlockedKeyPointIndices")
        .HasColumnName("UnlockedKeyPointIndices")
        .HasColumnType("jsonb");
    b.HasOne(te => te.Tour)
        .WithMany()
        .HasForeignKey(te => te.TourId)
        .OnDelete(DeleteBehavior.Restrict);
    b.HasIndex(te => new { te.TouristId, te.TourId, te.Status });
});
```

### API Layer (Explorer.API)

**TourExecutionController** - RESTful endpoints:
- `POST /api/tourist/tour-executions/start` - Pokreni turu
- `PUT /api/tourist/tour-executions/{id}/complete` - Završi turu
- `PUT /api/tourist/tour-executions/{id}/abandon` - Napusti turu
- `POST /api/tourist/tour-executions/{id}/unlock-keypoint` - Otključaj tačku
- `GET /api/tourist/tour-executions/active/{tourId}` - Aktivna sesija
- `GET /api/tourist/tour-executions/history` - Istorija izvršavanja
- `GET /api/tourist/tour-executions/{id}/keypoint/{index}/secret` - Pristup tajni

### DTOs

**StartTourExecutionDto**
```csharp
public class StartTourExecutionDto
{
    public long TourId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
```

**UnlockKeyPointDto**
```csharp
public class UnlockKeyPointDto
{
    public int KeyPointIndex { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
```

**TourExecutionDto**
```csharp
public class TourExecutionDto
{
    public long Id { get; set; }
    public long TouristId { get; set; }
    public long TourId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? AbandonedAt { get; set; }
    public string Status { get; set; }
    public double StartLatitude { get; set; }
    public double StartLongitude { get; set; }
    public List<int> UnlockedKeyPointIndices { get; set; }
    public TourDto? Tour { get; set; }
}
```

## Clean Code principi primenjeni

1. **Single Responsibility Principle (SRP)**
   - TourExecution entity upravlja samo stanjem izvršavanja ture
   - Service sloj upravlja orchestracijom i biznis pravilima
   - Repository upravlja perzistencijom

2. **Dependency Inversion Principle (DIP)**
   - Sve zavisnosti su kroz interfejse
   - Repository i Service su registrovani u DI containeru

3. **Open/Closed Principle (OCP)**
   - Domain entiteti su zatvoreni za modifikaciju ali otvoreni za ekstenziju
   - Nove statuse ili behavior može lako dodati bez izmene postojećeg koda

4. **Encapsulation**
   - Private setteri za business-critical properties
   - Read-only kolekc ije (IReadOnlyList) za izlaganje liste otključanih tačaka
   - Private business methods (Complete(), Abandon(), UnlockKeyPoint())

5. **Validation u domenu**
   - Validacija koordinata u konstruktoru
   - Validacija state transicija u metodama

## Kako koristiti (primeri)

### 1. Pokretanje ture
```http
POST /api/tourist/tour-executions/start
Authorization: Bearer {token}
Content-Type: application/json

{
  "tourId": 1,
  "latitude": 45.2551,
  "longitude": 19.8636
}
```

### 2. Otključavanje ključne tačke
```http
POST /api/tourist/tour-executions/{executionId}/unlock-keypoint
Authorization: Bearer {token}
Content-Type: application/json

{
  "keyPointIndex": 0,
  "latitude": 45.2552,
  "longitude": 19.8637
}
```

### 3. Pristup tajni
```http
GET /api/tourist/tour-executions/{executionId}/keypoint/{index}/secret
Authorization: Bearer {token}
```

Response:
```json
{
  "secret": "Tajna sadržina ključne tačke..."
}
```

### 4. Završavanje ture
```http
PUT /api/tourist/tour-executions/{executionId}/complete
Authorization: Bearer {token}
```

## Potrebna migracija

Za aktiviranje feature-a potrebno je pokrenuti EF Core migraciju:

```bash
cd Modules/Tours/Explorer.Tours.Infrastructure
dotnet ef migrations add AddTourExecution --context ToursContext
dotnet ef database update --context ToursContext
```

Ili ručno kreirati SQL:

```sql
CREATE TABLE tours."TourExecutions" (
    "Id" bigserial PRIMARY KEY,
    "TouristId" bigint NOT NULL,
    "TourId" bigint NOT NULL,
    "StartedAt" timestamp with time zone NOT NULL,
    "CompletedAt" timestamp with time zone,
    "AbandonedAt" timestamp with time zone,
    "Status" integer NOT NULL,
    "StartLatitude" double precision NOT NULL,
    "StartLongitude" double precision NOT NULL,
    "UnlockedKeyPointIndices" jsonb NOT NULL,
    CONSTRAINT "FK_TourExecutions_Tours_TourId" FOREIGN KEY ("TourId") 
        REFERENCES tours."Tours" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_TourExecutions_TouristId" ON tours."TourExecutions" ("TouristId");
CREATE INDEX "IX_TourExecutions_TouristId_TourId_Status" 
    ON tours."TourExecutions" ("TouristId", "TourId", "Status");
CREATE INDEX "IX_TourExecutions_TourId" ON tours."TourExecutions" ("TourId");
```

## Testing

Za testiranje implementirane funkcionalnosti:

1. Kupiti turu kroz ShoppingCart
2. Startovati tour execution sa validnom lokacijom
3. Proveriti da se ne može startovati dupla sesija za istu turu
4. Proveriti proximity check za otključavanje tačaka (< 100m)
5. Proveriti da se tajna ne može pristupiti pre otključavanja
6. Testirati complete/abandon scenarije

## Tehnički detalji

- **.NET 8** Target Framework
- **PostgreSQL** za perzistenciju sa JSONB podrškom
- **AutoMapper** za DTO mapiranja
- **Entity Framework Core** za ORM
- **JWT Authorization** za autentifikaciju

## Fajlovi kreirani/modifikovani

### Novi fajlovi:
- `Modules/Tours/Explorer.Tours.Core/Domain/TourExecution.cs`
- `Modules/Tours/Explorer.Tours.Core/Domain/RepositoryInterfaces/ITourExecutionRepository.cs`
- `Modules/Tours/Explorer.Tours.API/Dtos/TourExecutionDto.cs`
- `Modules/Tours/Explorer.Tours.API/Dtos/StartTourExecutionDto.cs`
- `Modules/Tours/Explorer.Tours.API/Dtos/UnlockKeyPointDto.cs`
- `Modules/Tours/Explorer.Tours.API/Public/Tourist/ITourExecutionService.cs`
- `Modules/Tours/Explorer.Tours.Core/UseCases/Tourist/TourExecutionService.cs`
- `Modules/Tours/Explorer.Tours.Infrastructure/Database/Repositories/TourExecutionDbRepository.cs`
- `Explorer.API/Controllers/Tourist/TourExecutionController.cs`

### Modifikovani fajlovi:
- `Modules/Tours/Explorer.Tours.Core/Mappers/ToursProfile.cs` (dodati mappings)
- `Modules/Tours/Explorer.Tours.Infrastructure/Database/ToursContext.cs` (DbSet i konfiguracija)
- `Modules/Tours/Explorer.Tours.Infrastructure/ToursStartup.cs` (DI registracija)

Build je uspešan ✓
