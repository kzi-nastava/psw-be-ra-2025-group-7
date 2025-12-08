# ?? Swagger Test Zahtevi - Follower System

## ?? Priprema: Login

```http
POST /api/users/login
Content-Type: application/json

{
  "username": "turista1@gmail.com",
  "password": "turista1"
}
```

Kopiraj `accessToken` i klikni **Authorize** u Swagger UI.

---

## 1?? FOLLOWER MANAGEMENT

### ? Prati korisnika (Follow)
```http
POST /api/followers/follow/{followedId}
Authorization: Bearer {token}

Primer: POST /api/followers/follow/-22
```

**Oèekivani rezultat:**
```json
{
  "id": 1,
  "followerId": -21,
  "followedId": -22,
  "followedAt": "2024-11-18T12:00:00Z"
}
```

---

### ? Prestani pratiti (Unfollow)
```http
DELETE /api/followers/unfollow/{followedId}
Authorization: Bearer {token}

Primer: DELETE /api/followers/unfollow/-22
```

**Oèekivani rezultat:** `200 OK`

---

### ? Pregled mojih pratilaca (Who follows me)
```http
GET /api/followers/my-followers?page=1&pageSize=20
Authorization: Bearer {token}
```

**Oèekivani rezultat:**
```json
{
  "results": [
    {
      "id": -3,
      "followerId": -22,
      "followedId": -21,
      "followedAt": "2024-02-01T09:00:00Z"
    }
  ],
  "totalCount": 2
}
```

---

### ? Pregled koga pratim (Who I'm following)
```http
GET /api/followers/my-following?page=1&pageSize=20
Authorization: Bearer {token}
```

**Oèekivani rezultat:**
```json
{
  "results": [
    {
      "id": -1,
      "followerId": -21,
      "followedId": -22,
      "followedAt": "2024-01-15T10:00:00Z"
    }
  ],
  "totalCount": 2
}
```

---

### ? Proveri da li pratim korisnika
```http
GET /api/followers/is-following/{followedId}
Authorization: Bearer {token}

Primer: GET /api/followers/is-following/-22
```

**Oèekivani rezultat:**
```json
true
```

---

## 2?? FOLLOWER MESSAGES (Poruke pratiocima)

### ? Pošalji poruku svim pratiocima
```http
POST /api/follower-messages
Authorization: Bearer {token}
Content-Type: application/json

{
  "content": "Pozdrav svima! Upravo sam objavio novu turu!",
  "resourceId": 1,
  "resourceType": "Tour"
}
```

**Primer bez resource linka:**
```json
{
  "content": "Hvala vam svima na podršci! ??"
}
```

**Primer sa blog post linkom:**
```json
{
  "content": "Pogledajte moj najnoviji blog post!",
  "resourceId": 5,
  "resourceType": "BlogPost"
}
```

**Oèekivani rezultat:**
```json
{
  "id": 1,
  "authorId": -21,
  "content": "Pozdrav svima! Upravo sam objavio novu turu!",
  "createdAt": "2024-11-18T12:30:00Z",
  "resourceId": 1,
  "resourceType": "Tour"
}
```

---

### ? Pregled mojih poruka za pratioce
```http
GET /api/follower-messages/my-messages?page=1&pageSize=20
Authorization: Bearer {token}
```

**Oèekivani rezultat:**
```json
{
  "results": [
    {
      "id": -1,
      "authorId": -21,
      "content": "Pozdrav svima! Upravo sam objavio novu turu!",
      "createdAt": "2024-03-01T10:00:00Z",
      "resourceId": -1,
      "resourceType": "Tour"
    }
  ],
  "totalCount": 2
}
```

---

### ? Obriši poruku
```http
DELETE /api/follower-messages/{messageId}
Authorization: Bearer {token}

Primer: DELETE /api/follower-messages/-1
```

**Oèekivani rezultat:** `200 OK`

---

## 3?? CLUB MESSAGES (Poruke na stranici kluba)

### ? Kreiraj poruku na stranici kluba
```http
POST /api/club-messages
Authorization: Bearer {token}
Content-Type: application/json

{
  "clubId": 1,
  "content": "Dobrodošli u naš klub! ??"
}
```

**Primer sa resource linkom:**
```json
{
  "clubId": 1,
  "content": "Nova tura dostupna za èlanove kluba!",
  "resourceId": 2,
  "resourceType": "Tour"
}
```

**Oèekivani rezultat:**
```json
{
  "id": 1,
  "clubId": 1,
  "authorId": -21,
  "content": "Dobrodošli u naš klub! ??",
  "createdAt": "2024-11-18T12:45:00Z",
  "updatedAt": null,
  "resourceId": null,
  "resourceType": null
}
```

---

### ? Ažuriraj poruku (samo autor)
```http
PUT /api/club-messages/{messageId}
Authorization: Bearer {token}
Content-Type: application/json

{
  "clubId": 1,
  "content": "Ažurirano: Informacije o sledeæem sastanku",
  "resourceId": null,
  "resourceType": null
}
```

**Oèekivani rezultat:**
```json
{
  "id": 1,
  "clubId": 1,
  "authorId": -21,
  "content": "Ažurirano: Informacije o sledeæem sastanku",
  "createdAt": "2024-11-18T12:45:00Z",
  "updatedAt": "2024-11-18T13:00:00Z",
  "resourceId": null,
  "resourceType": null
}
```

---

### ? Obriši poruku (samo vlasnik kluba)
```http
DELETE /api/club-messages/{messageId}
Authorization: Bearer {token}

Primer: DELETE /api/club-messages/-1
```

**Oèekivani rezultat:** `200 OK`

---

### ? Pregled poruka na stranici kluba
```http
GET /api/club-messages/club/{clubId}?page=1&pageSize=20
Authorization: Bearer {token}

Primer: GET /api/club-messages/club/1?page=1&pageSize=20
```

**Oèekivani rezultat:**
```json
{
  "results": [
    {
      "id": -1,
      "clubId": -1,
      "authorId": -21,
      "content": "Dobrodošli u naš klub! ??",
      "createdAt": "2024-03-01T10:00:00Z",
      "updatedAt": null,
      "resourceId": null,
      "resourceType": null
    }
  ],
  "totalCount": 3
}
```

---

## ?? Test Scenariji

### Scenario 1: Follow/Unfollow korisnika
1. ? POST `/api/followers/follow/-22` (Prati turista -22)
2. ? GET `/api/followers/is-following/-22` (Proveri - oèekuje `true`)
3. ? GET `/api/followers/my-following` (Vidi listu)
4. ? DELETE `/api/followers/unfollow/-22` (Prestani pratiti)
5. ? GET `/api/followers/is-following/-22` (Proveri - oèekuje `false`)

### Scenario 2: Slanje poruke pratiocima
1. ? POST `/api/followers/follow/-22` (Osiguraj da imaš pratioce)
2. ? POST `/api/follower-messages` sa telom:
   ```json
   {
     "content": "Test poruka!",
     "resourceId": 1,
     "resourceType": "Tour"
   }
   ```
3. ? GET `/api/follower-messages/my-messages` (Proveri kreiranu poruku)
4. ? DELETE `/api/follower-messages/{id}` (Obriši poruku)

### Scenario 3: Poruke na stranici kluba
1. ? POST `/api/club-messages` (Kreiraj poruku)
2. ? GET `/api/club-messages/club/1` (Pregled svih poruka)
3. ? PUT `/api/club-messages/{id}` (Ažuriraj poruku ako si autor)
4. ? DELETE `/api/club-messages/{id}` (Obriši ako si vlasnik kluba)

---

## ?? Validacije i Greške

### 400 Bad Request
- Content prazan ili preko 280 karaktera
- ResourceId bez ResourceType ili obratno
- Pokušaj follow samog sebe

### 401 Unauthorized
- Nedostaje JWT token
- Token je istekao

### 403 Forbidden
- Pokušaj ažuriranja tuðe poruke
- Pokušaj brisanja poruke ako nisi vlasnik kluba

### 404 Not Found
- Korisnik ne postoji
- Poruka ne postoji
- Klub ne postoji

---

## ?? Validacija u bazi

```sql
-- Proveri followers
SELECT * FROM stakeholders."Followers" WHERE "FollowerId" = -21;

-- Proveri follower messages
SELECT * FROM stakeholders."FollowerMessages" ORDER BY "CreatedAt" DESC;

-- Proveri club messages
SELECT * FROM stakeholders."ClubMessages" WHERE "ClubId" = -1 ORDER BY "CreatedAt" DESC;

-- Proveri notifikacije (PLACEHOLDER - za taèku 3)
SELECT * FROM stakeholders."Notifications" WHERE "UserId" = -21 AND "IsRead" = false;
```
