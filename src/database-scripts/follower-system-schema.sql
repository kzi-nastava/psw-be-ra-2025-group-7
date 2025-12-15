-- ========================================
-- TABELA: stakeholders.Followers
-- ========================================
CREATE TABLE IF NOT EXISTS stakeholders."Followers" (
    "Id" bigserial PRIMARY KEY,
    "FollowerId" bigint NOT NULL,
    "FollowedId" bigint NOT NULL,
    "FollowedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "FK_Followers_People_FollowerId" FOREIGN KEY ("FollowerId") 
        REFERENCES stakeholders."People"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Followers_People_FollowedId" FOREIGN KEY ("FollowedId") 
        REFERENCES stakeholders."People"("Id") ON DELETE CASCADE,
    CONSTRAINT "UQ_Followers_FollowerId_FollowedId" UNIQUE ("FollowerId", "FollowedId"),
    CONSTRAINT "CK_Followers_NotSelf" CHECK ("FollowerId" != "FollowedId")
);

CREATE INDEX "IX_Followers_FollowedId" ON stakeholders."Followers" ("FollowedId");
CREATE INDEX "IX_Followers_FollowerId" ON stakeholders."Followers" ("FollowerId");

COMMENT ON TABLE stakeholders."Followers" IS 'Relacija pratilac-praæeni';
COMMENT ON COLUMN stakeholders."Followers"."FollowerId" IS 'Person.Id koji prati';
COMMENT ON COLUMN stakeholders."Followers"."FollowedId" IS 'Person.Id koji je praæen';

-- ========================================
-- TABELA: stakeholders.FollowerMessages
-- ========================================
CREATE TABLE IF NOT EXISTS stakeholders."FollowerMessages" (
    "Id" bigserial PRIMARY KEY,
    "AuthorId" bigint NOT NULL,
    "Content" character varying(280) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "ResourceId" bigint NULL,
    "ResourceType" integer NULL,
    CONSTRAINT "FK_FollowerMessages_People_AuthorId" FOREIGN KEY ("AuthorId") 
        REFERENCES stakeholders."People"("Id") ON DELETE CASCADE,
    CONSTRAINT "CK_FollowerMessages_ResourceConsistency" 
        CHECK (("ResourceId" IS NULL AND "ResourceType" IS NULL) OR 
               ("ResourceId" IS NOT NULL AND "ResourceType" IS NOT NULL))
);

CREATE INDEX "IX_FollowerMessages_AuthorId" ON stakeholders."FollowerMessages" ("AuthorId");
CREATE INDEX "IX_FollowerMessages_CreatedAt" ON stakeholders."FollowerMessages" ("CreatedAt" DESC);

COMMENT ON TABLE stakeholders."FollowerMessages" IS 'Poruke koje korisnici šalju svojim pratiocima';
COMMENT ON COLUMN stakeholders."FollowerMessages"."ResourceType" IS '0=Tour, 1=BlogPost';

-- ========================================
-- TABELA: stakeholders.ClubMessages
-- ========================================
CREATE TABLE IF NOT EXISTS stakeholders."ClubMessages" (
    "Id" bigserial PRIMARY KEY,
    "ClubId" bigint NOT NULL,
    "AuthorId" bigint NOT NULL,
    "Content" character varying(280) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NULL,
    "ResourceId" bigint NULL,
    "ResourceType" integer NULL,
    CONSTRAINT "FK_ClubMessages_Clubs_ClubId" FOREIGN KEY ("ClubId") 
        REFERENCES stakeholders."Clubs"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ClubMessages_People_AuthorId" FOREIGN KEY ("AuthorId") 
        REFERENCES stakeholders."People"("Id") ON DELETE CASCADE,
    CONSTRAINT "CK_ClubMessages_ResourceConsistency" 
        CHECK (("ResourceId" IS NULL AND "ResourceType" IS NULL) OR 
               ("ResourceId" IS NOT NULL AND "ResourceType" IS NOT NULL"))
);

CREATE INDEX "IX_ClubMessages_ClubId" ON stakeholders."ClubMessages" ("ClubId");
CREATE INDEX "IX_ClubMessages_AuthorId" ON stakeholders."ClubMessages" ("AuthorId");
CREATE INDEX "IX_ClubMessages_CreatedAt" ON stakeholders."ClubMessages" ("CreatedAt" DESC);

COMMENT ON TABLE stakeholders."ClubMessages" IS 'Poruke na stranici kluba';
COMMENT ON COLUMN stakeholders."ClubMessages"."ResourceType" IS '0=Tour, 1=BlogPost';

-- ========================================
-- TABELA: stakeholders.Notifications (PLACEHOLDER)
-- ========================================
CREATE TABLE IF NOT EXISTS stakeholders."Notifications" (
    "Id" bigserial PRIMARY KEY,
    "UserId" bigint NOT NULL,
    "Type" integer NOT NULL,
    "Content" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "IsRead" boolean NOT NULL DEFAULT false,
    "ResourceId" bigint NULL,
    "ResourceType" integer NULL,
    "SourceFollowerMessageId" bigint NULL,
    "SourceClubMessageId" bigint NULL,
    CONSTRAINT "FK_Notifications_People_UserId" FOREIGN KEY ("UserId") 
        REFERENCES stakeholders."People"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Notifications_FollowerMessages" FOREIGN KEY ("SourceFollowerMessageId") 
        REFERENCES stakeholders."FollowerMessages"("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Notifications_ClubMessages" FOREIGN KEY ("SourceClubMessageId") 
        REFERENCES stakeholders."ClubMessages"("Id") ON DELETE SET NULL
);

CREATE INDEX "IX_Notifications_UserId_IsRead" ON stakeholders."Notifications" ("UserId", "IsRead");
CREATE INDEX "IX_Notifications_CreatedAt" ON stakeholders."Notifications" ("CreatedAt" DESC);

COMMENT ON TABLE stakeholders."Notifications" IS 'PLACEHOLDER - Sistem notifikacija za taèku (3)';
COMMENT ON COLUMN stakeholders."Notifications"."Type" IS '0=FollowerMessage, 1=ClubActivity';

-- ========================================
-- TEST PODACI
-- ========================================

-- Test followers (pretpostavljamo da veæ postoje Person ID: -21, -22, -23)
INSERT INTO stakeholders."Followers"("Id", "FollowerId", "FollowedId", "FollowedAt")
VALUES 
    (-1, -21, -22, '2024-01-15 10:00:00+00'),  -- Turista -21 prati turista -22
    (-2, -21, -23, '2024-01-20 11:00:00+00'),  -- Turista -21 prati turista -23
    (-3, -22, -21, '2024-02-01 09:00:00+00'),  -- Turista -22 prati turista -21
    (-4, -23, -21, '2024-02-10 14:00:00+00')   -- Turista -23 prati turista -21
ON CONFLICT ("Id") DO NOTHING;

-- Test follower messages
INSERT INTO stakeholders."FollowerMessages"("Id", "AuthorId", "Content", "CreatedAt", "ResourceId", "ResourceType")
VALUES 
    (-1, -21, 'Pozdrav svima! Upravo sam objavio novu turu!', '2024-03-01 10:00:00+00', -1, 0),  -- Link ka Tour ID -1
    (-2, -22, 'Pogledajte moj najnoviji blog post o putovanju!', '2024-03-05 15:30:00+00', -1, 1),  -- Link ka BlogPost ID -1
    (-3, -21, 'Hvala vam svima na podršci! ??', '2024-03-10 12:00:00+00', NULL, NULL)  -- Bez linka
ON CONFLICT ("Id") DO NOTHING;

-- Test club messages (pretpostavljamo da postoji Club ID: -1)
INSERT INTO stakeholders."ClubMessages"("Id", "ClubId", "AuthorId", "Content", "CreatedAt", "UpdatedAt", "ResourceId", "ResourceType")
VALUES 
    (-1, -1, -21, 'Dobrodošli u naš klub! ??', '2024-03-01 10:00:00+00', NULL, NULL, NULL),
    (-2, -1, -22, 'Nova tura dostupna za èlanove kluba!', '2024-03-05 14:00:00+00', NULL, -2, 0),  -- Link ka Tour ID -2
    (-3, -1, -21, 'Ažurirano: Informacije o sledeæem sastanku', '2024-03-08 09:00:00+00', '2024-03-08 11:00:00+00', NULL, NULL)
ON CONFLICT ("Id") DO NOTHING;

-- Ažuriranje sequences
SELECT setval('stakeholders."Followers_Id_seq"', 100, true);
SELECT setval('stakeholders."FollowerMessages_Id_seq"', 100, true);
SELECT setval('stakeholders."ClubMessages_Id_seq"', 100, true);
SELECT setval('stakeholders."Notifications_Id_seq"', 100, true);
