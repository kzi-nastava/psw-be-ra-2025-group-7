# ?? Follower System - Kompletan pregled implementacije

## ? **Implementirane funkcionalnosti**

### **(1) Praæenje korisnika i poruke pratiocima**
- ? Korisnik može videti svoje pratioce na profilu
- ? Korisnik može poslati poruku svim pratiocima (max 280 karaktera)
- ? Uz poruku može ataèovati link ka Tour ili BlogPost resursu
- ? Resource linking preko ID-a i ResourceType enum-a (ne fizièki URL)

### **(2) Poruke na stranici kluba**
- ? Èlanovi kluba mogu postavljati poruke na stranicu kluba
- ? Èlanovi mogu menjati svoje poruke
- ? Vlasnik kluba može brisati sve poruke
- ? Uz poruku može ataèovati link ka resursu

### **(3) PLACEHOLDER: Notifikacije**
- ? Database tabela kreirana
- ? Domain entitet spreman
- ? TODO komentari u servisu za buduæu implementaciju
- ? Service i Controller trebaju biti implementirani kasnije

---

## ??? **Arhitektura i DDD pristup**

### **Aggregate Roots**
1. **FollowerMessage** - Poruka pratiocima (autor + sadržaj + resource)
2. **ClubMessage** - Poruka na stranici kluba (klub + autor + sadržaj + resource)

### **Entities**
1. **Follower** - Relacija pratilac-praæeni (jednostavna veza bez dodatne logike)
2. **Notification** - Placeholder za sistem notifikacija

### **Value Objects / Enums**
- **ResourceType** - Enum (Tour = 0, BlogPost = 1)
- **NotificationType** - Enum (FollowerMessage = 0, ClubActivity = 1)

---

## ?? **Kreirani fajlovi**

### **Domain Layer (Core)**
```
Modules\Stakeholders\Explorer.Stakeholders.Core\Domain\
??? Follower.cs
??? FollowerMessage.cs (Aggregate Root)
??? ClubMessage.cs (Aggregate Root)
??? Notification.cs (PLACEHOLDER)
??? ResourceType.cs (Enum)

Modules\Stakeholders\Explorer.Stakeholders.Core\Domain\RepositoryInterfaces\
??? IFollowerRepository.cs
??? IFollowerMessageRepository.cs
??? IClubMessageRepository.cs
```

### **Application Layer (API + Core)**
```
Modules\Stakeholders\Explorer.Stakeholders.API\Dtos\
??? FollowerDto.cs
??? FollowerMessageDto.cs
??? ClubMessageDto.cs

Modules\Stakeholders\Explorer.Stakeholders.API\Public\
??? IFollowerService.cs
??? IFollowerMessageService.cs
??? IClubMessageService.cs

Modules\Stakeholders\Explorer.Stakeholders.Core\UseCases\
??? FollowerService.cs
??? FollowerMessageService.cs
??? ClubMessageService.cs

Modules\Stakeholders\Explorer.Stakeholders.Core\Mappers\
??? StakeholderProfile.cs (updated)
```

### **Infrastructure Layer**
```
Modules\Stakeholders\Explorer.Stakeholders.Infrastructure\Database\Repositories\
??? FollowerDbRepository.cs
??? FollowerMessageDbRepository.cs
??? ClubMessageDbRepository.cs

Modules\Stakeholders\Explorer.Stakeholders.Infrastructure\Database\
??? StakeholdersContext.cs (updated)
??? StakeholdersStartup.cs (updated)
```

### **Presentation Layer (API)**
```
Explorer.API\Controllers\
??? Follower\
?   ??? FollowerController.cs
?   ??? FollowerMessageController.cs
??? Club\
    ??? ClubMessageController.cs
```

### **Database & Documentation**
```
database-scripts\
??? follower-system-schema.sql

documentation\
??? swagger-test-requests.md
??? frontend-integration-guide.md
```

---

## ??? **Database Schema**

### **Tabele kreirane:**
1. `stakeholders.Followers` - Relacija pratilac-praæeni
2. `stakeholders.FollowerMessages` - Poruke pratiocima
3. `stakeholders.ClubMessages` - Poruke na stranici kluba
4. `stakeholders.Notifications` - PLACEHOLDER za notifikacije

### **Key Constraints:**
- Unique constraint: (`FollowerId`, `FollowedId`) - Ne može duplikat follow
- Check constraint: `FollowerId != FollowedId` - Ne može pratiti sebe
- Check constraint: Resource consistency - ResourceId i ResourceType oba ili nijedno

### **Indexes:**
- `IX_Followers_FollowedId`, `IX_Followers_FollowerId` - Brži upiti
- `IX_FollowerMessages_AuthorId`, `IX_FollowerMessages_CreatedAt`
- `IX_ClubMessages_ClubId`, `IX_ClubMessages_AuthorId`
- `IX_Notifications_UserId_IsRead` - Za buduæe notifikacije

---

## ?? **API Endpoints**

### **Follower Management**
| Method | Endpoint | Opis |
|--------|----------|------|
| POST | `/api/followers/follow/{id}` | Prati korisnika |
| DELETE | `/api/followers/unfollow/{id}` | Prestani pratiti |
| GET | `/api/followers/my-followers` | Moji pratioci |
| GET | `/api/followers/my-following` | Koga pratim |
| GET | `/api/followers/is-following/{id}` | Da li pratim |

### **Follower Messages**
| Method | Endpoint | Opis |
|--------|----------|------|
| POST | `/api/follower-messages` | Pošalji poruku pratiocima |
| GET | `/api/follower-messages/my-messages` | Moje poruke |
| DELETE | `/api/follower-messages/{id}` | Obriši poruku |

### **Club Messages**
| Method | Endpoint | Opis |
|--------|----------|------|
| POST | `/api/club-messages` | Kreiraj poruku (èlan kluba) |
| PUT | `/api/club-messages/{id}` | Ažuriraj poruku (autor) |
| DELETE | `/api/club-messages/{id}` | Obriši poruku (vlasnik kluba) |
| GET | `/api/club-messages/club/{clubId}` | Sve poruke kluba |

---

## ?? **Autorizacija**

- **Svi endpoints** zahtevaju `[Authorize(Policy = "touristPolicy")]`
- **PersonId** se automatski uzima iz JWT tokena (`User.PersonId()`)
- **Ownership validation:**
  - Follower messages: Samo autor može brisati
  - Club messages: Samo autor može ažurirati, samo vlasnik kluba može brisati

---

## ?? **Test podaci**

### **Followers** (4 test relacije)
- -21 prati -22
- -21 prati -23
- -22 prati -21
- -23 prati -21

### **FollowerMessages** (3 test poruke)
- Poruka sa Tour linkom
- Poruka sa BlogPost linkom
- Poruka bez linka

### **ClubMessages** (3 test poruke)
- Dobrodošlica bez linka
- Poruka sa Tour linkom
- Ažurirana poruka

---

## ?? **Dependency Injection**

```csharp
// Services
services.AddScoped<IFollowerService, FollowerService>();
services.AddScoped<IFollowerMessageService, FollowerMessageService>();
services.AddScoped<IClubMessageService, ClubMessageService>();

// Repositories
services.AddScoped<IFollowerRepository, FollowerDbRepository>();
services.AddScoped<IFollowerMessageRepository, FollowerMessageDbRepository>();
services.AddScoped<IClubMessageRepository, ClubMessageDbRepository>();
```

---

## ?? **Resource Linking Pattern**

```csharp
// Domain
public enum ResourceType { Tour = 0, BlogPost = 1 }

// Entity
public long? ResourceId { get; private set; }
public ResourceType? ResourceType { get; private set; }

// Validacija
if ((resourceId.HasValue && !resourceType.HasValue) || 
    (!resourceId.HasValue && resourceType.HasValue))
{
    throw new ArgumentException("Both or neither must be provided");
}

// DTO (string za jednostavniju JSON serijalizaciju)
public long? ResourceId { get; set; }
public string? ResourceType { get; set; }  // "Tour" ili "BlogPost"

// Mapping (AutoMapper)
.ForMember(dest => dest.ResourceType, 
    opt => opt.MapFrom(src => src.ResourceType.HasValue ? src.ResourceType.ToString() : null))
```

---

## ?? **Validacije**

### **FollowerMessage / ClubMessage**
- ? Content: Required, max 280 karaktera
- ? ResourceId i ResourceType: Oba ili nijedno
- ? AuthorId: Required, != 0

### **Follower**
- ? FollowerId i FollowedId: Required, != 0
- ? FollowerId != FollowedId (ne može pratiti sebe)
- ? Unique constraint (ne može duplirati follow)

---

## ?? **Kako pokrenuti**

### **1. Pokreni SQL skriptu**
```bash
psql -U postgres -d explorer-v1 -f database-scripts/follower-system-schema.sql
```

### **2. Pokreni aplikaciju**
```bash
dotnet run --project Explorer.API
```

### **3. Testiraj u Swagger-u**
```
https://localhost:5001/swagger
```

Prati korake iz `documentation/swagger-test-requests.md`

---

## ?? **Statistika**

| Metrika | Vrednost |
|---------|----------|
| **Novi entiteti** | 4 (Follower, FollowerMessage, ClubMessage, Notification) |
| **Novi servisi** | 3 (FollowerService, FollowerMessageService, ClubMessageService) |
| **Novi repozitorijumi** | 3 (FollowerDbRepository, FollowerMessageDbRepository, ClubMessageDbRepository) |
| **Novi kontroleri** | 3 (FollowerController, FollowerMessageController, ClubMessageController) |
| **API endpoints** | 11 |
| **Database tabele** | 4 |
| **DTO klase** | 3 |
| **Linije koda** | ~1500+ |

---

## ? **Build Status**

```
? Build successful
? No compilation errors
? All dependencies resolved
? Database schema validated
```

---

## ?? **TODO za taèku (3) - Notifikacije**

Za implementaciju sistema notifikacija potrebno je:

1. **Service:** `INotificationService` + `NotificationService`
2. **Repository:** `INotificationRepository` + `NotificationDbRepository`
3. **Controller:** `NotificationController`
4. **Endpoints:**
   - GET `/api/notifications` - Sve notifikacije
   - GET `/api/notifications/unread` - Neproèitane
   - PUT `/api/notifications/{id}/mark-as-read`
   - PUT `/api/notifications/mark-all-as-read`

5. **Integration:**
   - U `FollowerMessageService.SendMessageToFollowers()` dodati kreiranje notifikacija
   - U `ClubMessageService.CreateMessage()` dodati kreiranje notifikacija
   - Implementirati logiku za brisanje notifikacija kada se obriše izvor

6. **Real-time (opciono):**
   - SignalR hub za push notifikacije
   - WebSocket konekcija

---

## ?? **Zakljuèak**

Kompletan back-end sistem za praæenje korisnika, poruke pratiocima i poruke na stranici kluba je implementiran prema DDD principima sa:

- ? Èistom arhitekturom (Domain ? Application ? Infrastructure ? Presentation)
- ? Aggregate root pattern za kompleksnije entitete
- ? Repository pattern za data access
- ? Dependency injection za loose coupling
- ? Resource linking pattern bez fizièkih URL-ova
- ? Pripremljenom infrastrukturom za notifikacije
- ? Kompletnom dokumentacijom za front-end integraciju

**Build je uspešan! Sve je spremno za testiranje! ??**
