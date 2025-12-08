# ?? Front-End Integration Guide - Follower System

## ?? **Pregled API Endpoints-a**

### Base URL
```
https://localhost:5001/api
```

---

## 1?? **FOLLOWER MANAGEMENT**

### Follow User
```typescript
POST /followers/follow/{followedId}
Headers: {
  'Authorization': 'Bearer {accessToken}',
  'Content-Type': 'application/json'
}
```

**Response:**
```typescript
interface FollowerDto {
  id: number;
  followerId: number;
  followedId: number;
  followedAt: string; // ISO 8601 datetime
  followerName?: string;
  followerSurname?: string;
}
```

---

### Unfollow User
```typescript
DELETE /followers/unfollow/{followedId}
Headers: {
  'Authorization': 'Bearer {accessToken}'
}
```

**Response:** `200 OK`

---

### Get My Followers (Who follows me)
```typescript
GET /followers/my-followers?page=1&pageSize=20
Headers: {
  'Authorization': 'Bearer {accessToken}'
}
```

**Response:**
```typescript
interface PagedResult<T> {
  results: T[];
  totalCount: number;
}

// Returns PagedResult<FollowerDto>
```

---

### Get My Following (Who I'm following)
```typescript
GET /followers/my-following?page=1&pageSize=20
Headers: {
  'Authorization': 'Bearer {accessToken}'
}
```

**Response:** `PagedResult<FollowerDto>`

---

### Check if Following
```typescript
GET /followers/is-following/{followedId}
Headers: {
  'Authorization': 'Bearer {accessToken}'
}
```

**Response:** `boolean`

---

## 2?? **FOLLOWER MESSAGES**

### Send Message to Followers
```typescript
POST /follower-messages
Headers: {
  'Authorization': 'Bearer {accessToken}',
  'Content-Type': 'application/json'
}
Body: {
  content: string;           // Required, max 280 characters
  resourceId?: number;       // Optional - Tour or BlogPost ID
  resourceType?: 'Tour' | 'BlogPost';  // Required if resourceId is provided
}
```

**Response:**
```typescript
interface FollowerMessageDto {
  id: number;
  authorId: number;
  content: string;
  createdAt: string;
  resourceId?: number;
  resourceType?: 'Tour' | 'BlogPost';
  authorName?: string;
  authorSurname?: string;
}
```

---

### Get My Follower Messages
```typescript
GET /follower-messages/my-messages?page=1&pageSize=20
Headers: {
  'Authorization': 'Bearer {accessToken}'
}
```

**Response:** `PagedResult<FollowerMessageDto>`

---

### Delete Follower Message
```typescript
DELETE /follower-messages/{messageId}
Headers: {
  'Authorization': 'Bearer {accessToken}'
}
```

**Response:** `200 OK`

---

## 3?? **CLUB MESSAGES**

### Create Club Message
```typescript
POST /club-messages
Headers: {
  'Authorization': 'Bearer {accessToken}',
  'Content-Type': 'application/json'
}
Body: {
  clubId: number;            // Required
  content: string;           // Required, max 280 characters
  resourceId?: number;       // Optional
  resourceType?: 'Tour' | 'BlogPost';
}
```

**Response:**
```typescript
interface ClubMessageDto {
  id: number;
  clubId: number;
  authorId: number;
  content: string;
  createdAt: string;
  updatedAt?: string;
  resourceId?: number;
  resourceType?: 'Tour' | 'BlogPost';
  authorName?: string;
  authorSurname?: string;
  clubName?: string;
}
```

---

### Update Club Message
```typescript
PUT /club-messages/{messageId}
Headers: {
  'Authorization': 'Bearer {accessToken}',
  'Content-Type': 'application/json'
}
Body: {
  clubId: number;
  content: string;
  resourceId?: number;
  resourceType?: 'Tour' | 'BlogPost';
}
```

**Response:** `ClubMessageDto`

---

### Delete Club Message
```typescript
DELETE /club-messages/{messageId}
Headers: {
  'Authorization': 'Bearer {accessToken}'
}
```

**Response:** `200 OK`

---

### Get Club Messages
```typescript
GET /club-messages/club/{clubId}?page=1&pageSize=20
Headers: {
  'Authorization': 'Bearer {accessToken}'
}
```

**Response:** `PagedResult<ClubMessageDto>`

---

## ?? **Angular Service Primer**

```typescript
import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../environments/environment';

export interface FollowerDto {
  id: number;
  followerId: number;
  followedId: number;
  followedAt: string;
  followerName?: string;
  followerSurname?: string;
}

export interface FollowerMessageDto {
  id?: number;
  authorId?: number;
  content: string;
  createdAt?: string;
  resourceId?: number;
  resourceType?: 'Tour' | 'BlogPost';
  authorName?: string;
  authorSurname?: string;
}

export interface ClubMessageDto {
  id?: number;
  clubId: number;
  authorId?: number;
  content: string;
  createdAt?: string;
  updatedAt?: string;
  resourceId?: number;
  resourceType?: 'Tour' | 'BlogPost';
  authorName?: string;
  authorSurname?: string;
  clubName?: string;
}

export interface PagedResult<T> {
  results: T[];
  totalCount: number;
}

@Injectable({
  providedIn: 'root'
})
export class FollowerService {
  private baseUrl = `${environment.apiUrl}/followers`;
  private messageUrl = `${environment.apiUrl}/follower-messages`;
  private clubMessageUrl = `${environment.apiUrl}/club-messages`;

  constructor(private http: HttpClient) { }

  // ============ FOLLOWER MANAGEMENT ============
  
  follow(followedId: number): Observable<FollowerDto> {
    return this.http.post<FollowerDto>(`${this.baseUrl}/follow/${followedId}`, {});
  }

  unfollow(followedId: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/unfollow/${followedId}`);
  }

  getMyFollowers(page: number = 1, pageSize: number = 20): Observable<PagedResult<FollowerDto>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<PagedResult<FollowerDto>>(`${this.baseUrl}/my-followers`, { params });
  }

  getMyFollowing(page: number = 1, pageSize: number = 20): Observable<PagedResult<FollowerDto>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<PagedResult<FollowerDto>>(`${this.baseUrl}/my-following`, { params });
  }

  isFollowing(followedId: number): Observable<boolean> {
    return this.http.get<boolean>(`${this.baseUrl}/is-following/${followedId}`);
  }

  // ============ FOLLOWER MESSAGES ============

  sendMessageToFollowers(message: FollowerMessageDto): Observable<FollowerMessageDto> {
    return this.http.post<FollowerMessageDto>(this.messageUrl, message);
  }

  getMyFollowerMessages(page: number = 1, pageSize: number = 20): Observable<PagedResult<FollowerMessageDto>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<PagedResult<FollowerMessageDto>>(`${this.messageUrl}/my-messages`, { params });
  }

  deleteFollowerMessage(messageId: number): Observable<void> {
    return this.http.delete<void>(`${this.messageUrl}/${messageId}`);
  }

  // ============ CLUB MESSAGES ============

  createClubMessage(message: ClubMessageDto): Observable<ClubMessageDto> {
    return this.http.post<ClubMessageDto>(this.clubMessageUrl, message);
  }

  updateClubMessage(messageId: number, message: ClubMessageDto): Observable<ClubMessageDto> {
    return this.http.put<ClubMessageDto>(`${this.clubMessageUrl}/${messageId}`, message);
  }

  deleteClubMessage(messageId: number): Observable<void> {
    return this.http.delete<void>(`${this.clubMessageUrl}/${messageId}`);
  }

  getClubMessages(clubId: number, page: number = 1, pageSize: number = 20): Observable<PagedResult<ClubMessageDto>> {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());
    return this.http.get<PagedResult<ClubMessageDto>>(`${this.clubMessageUrl}/club/${clubId}`, { params });
  }
}
```

---

## ?? **React/Next.js Primer**

```typescript
// hooks/useFollowers.ts
import { useState, useEffect } from 'react';
import axios from 'axios';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL;

export interface FollowerDto {
  id: number;
  followerId: number;
  followedId: number;
  followedAt: string;
}

export const useFollowers = () => {
  const [followers, setFollowers] = useState<FollowerDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const follow = async (followedId: number) => {
    setLoading(true);
    try {
      const response = await axios.post(
        `${API_BASE_URL}/followers/follow/${followedId}`,
        {},
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem('accessToken')}`
          }
        }
      );
      return response.data;
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to follow user');
      throw err;
    } finally {
      setLoading(false);
    }
  };

  const unfollow = async (followedId: number) => {
    setLoading(true);
    try {
      await axios.delete(
        `${API_BASE_URL}/followers/unfollow/${followedId}`,
        {
          headers: {
            Authorization: `Bearer ${localStorage.getItem('accessToken')}`
          }
        }
      );
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to unfollow user');
      throw err;
    } finally {
      setLoading(false);
    }
  };

  const getMyFollowers = async (page: number = 1, pageSize: number = 20) => {
    setLoading(true);
    try {
      const response = await axios.get(
        `${API_BASE_URL}/followers/my-followers`,
        {
          params: { page, pageSize },
          headers: {
            Authorization: `Bearer ${localStorage.getItem('accessToken')}`
          }
        }
      );
      setFollowers(response.data.results);
      return response.data;
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to fetch followers');
      throw err;
    } finally {
      setLoading(false);
    }
  };

  return { followers, loading, error, follow, unfollow, getMyFollowers };
};
```

---

## ?? **Error Handling**

```typescript
// error-handler.ts
export interface ApiError {
  status: number;
  message: string;
  errors?: Record<string, string[]>;
}

export const handleApiError = (error: any): ApiError => {
  if (error.response) {
    const status = error.response.status;
    const message = error.response.data?.message || error.message;
    
    switch (status) {
      case 400:
        return { status, message: 'Invalid request data' };
      case 401:
        return { status, message: 'Authentication required' };
      case 403:
        return { status, message: 'You do not have permission' };
      case 404:
        return { status, message: 'Resource not found' };
      default:
        return { status, message };
    }
  }
  
  return { status: 0, message: 'Network error' };
};
```

---

## ?? **PLACEHOLDER: Notifications (Taèka 3)**

Za taèku 3 æe biti potrebno:

1. **GET** `/api/notifications` - Preuzmi sve notifikacije
2. **GET** `/api/notifications/unread` - Samo neproèitane
3. **PUT** `/api/notifications/{id}/mark-as-read` - Oznaèi kao proèitano
4. **PUT** `/api/notifications/mark-all-as-read` - Sve kao proèitane

```typescript
// PLACEHOLDER interface
export interface NotificationDto {
  id: number;
  userId: number;
  type: 'FollowerMessage' | 'ClubActivity';
  content: string;
  createdAt: string;
  isRead: boolean;
  resourceId?: number;
  resourceType?: 'Tour' | 'BlogPost';
  sourceFollowerMessageId?: number;
  sourceClubMessageId?: number;
}
```

---

## ?? **Usage Examples**

### Follow/Unfollow Button Component
```typescript
// FollowButton.tsx
import { useState } from 'react';
import { useFollowers } from '../hooks/useFollowers';

export const FollowButton = ({ userId }: { userId: number }) => {
  const [isFollowing, setIsFollowing] = useState(false);
  const { follow, unfollow, loading } = useFollowers();

  const handleToggleFollow = async () => {
    try {
      if (isFollowing) {
        await unfollow(userId);
        setIsFollowing(false);
      } else {
        await follow(userId);
        setIsFollowing(true);
      }
    } catch (error) {
      console.error('Failed to toggle follow', error);
    }
  };

  return (
    <button onClick={handleToggleFollow} disabled={loading}>
      {loading ? 'Loading...' : isFollowing ? 'Unfollow' : 'Follow'}
    </button>
  );
};
```

---

## ?? **Resource Type Enum**

```typescript
export enum ResourceType {
  Tour = 'Tour',
  BlogPost = 'BlogPost'
}

// Helper function
export const getResourceUrl = (resourceId: number, resourceType: ResourceType): string => {
  switch (resourceType) {
    case ResourceType.Tour:
      return `/tours/${resourceId}`;
    case ResourceType.BlogPost:
      return `/blog/${resourceId}`;
    default:
      return '#';
  }
};
```
