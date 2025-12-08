INSERT INTO "stakeholders"."Clubs" 
("Id", "Name", "Description", "CreatedBy", "ImageUrls", "CreatedAt", "UpdatedAt")
VALUES
    (-1, 'ALamos planinarski klub', 'Klub za planinarske ture', -21, ARRAY['slika1.jpg'], NOW(), NOW()),
    (-2, 'Danubius veslacki klub', 'Klub za veslacke ture', -21, ARRAY['slika2.jpg'], NOW(), NOW()),
    (-3, 'Extreme Summit Klub', 'Klub za alpinisticke ture', -21, ARRAY['slika3.jpg'], NOW(), NOW())
ON CONFLICT ("Id") DO NOTHING;
