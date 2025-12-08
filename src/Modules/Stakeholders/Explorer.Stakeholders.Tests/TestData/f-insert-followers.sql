-- Test data for Followers
INSERT INTO stakeholders."Followers"("Id", "FollowerId", "FollowedId", "FollowedAt")
VALUES 
    (-1, -21, -22, '2024-01-15 10:00:00+00'),
    (-2, -21, -23, '2024-01-20 11:00:00+00'),
    (-3, -22, -21, '2024-02-01 09:00:00+00'),
    (-4, -23, -21, '2024-02-10 14:00:00+00')
ON CONFLICT ("Id") DO NOTHING;

-- Test data for FollowerMessages
INSERT INTO stakeholders."FollowerMessages"("Id", "AuthorId", "Content", "CreatedAt", "ResourceId", "ResourceType")
VALUES 
    (-1, -21, 'Pozdrav svima! Upravo sam objavio novu turu!', '2024-03-01 10:00:00+00', -1, 0),
    (-2, -22, 'Pogledajte moj najnoviji blog post o putovanju!', '2024-03-05 15:30:00+00', -1, 1),
    (-3, -21, 'Hvala vam svima na podršci!', '2024-03-10 12:00:00+00', NULL, NULL)
ON CONFLICT ("Id") DO NOTHING;

-- Test data for ClubMessages (Club with Id=-1 exists from d-clubs.sql)
INSERT INTO stakeholders."ClubMessages"("Id", "ClubId", "AuthorId", "Content", "CreatedAt", "UpdatedAt", "ResourceId", "ResourceType")
VALUES 
    (-1, -1, -21, 'Dobrodošli u naš klub!', '2024-03-01 10:00:00+00', NULL, NULL, NULL),
    (-2, -1, -22, 'Nova tura dostupna za èlanove kluba!', '2024-03-05 14:00:00+00', NULL, -2, 0),
    (-3, -1, -21, 'Ažurirano: Informacije o sledeæem sastanku', '2024-03-08 09:00:00+00', '2024-03-08 11:00:00+00', NULL, NULL)
ON CONFLICT ("Id") DO NOTHING;
