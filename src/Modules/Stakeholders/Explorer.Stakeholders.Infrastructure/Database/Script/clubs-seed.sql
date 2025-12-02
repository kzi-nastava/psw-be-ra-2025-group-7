INSERT INTO stakeholders."Clubs" ("Id", "Name", "Description", "CreatedBy", "ImageUrls", "CreatedAt", "UpdatedAt")
VALUES
(-10, 'test klub', 'opis test klub', -21, ARRAY['slika.jpg'], NOW(), NOW()),
(1, 'AlmaMons planinarski klub', 'Klub za planinarske ture', -21, ARRAY['slika1.jpg'], NOW(), NOW()),
(2, 'Dunubius veslački klub', 'Klub za veslacke ture', -21, ARRAY['slika2.jpg'], NOW(), NOW()),
(3, 'Extreme Summit Klub', 'Klub za alpinističke ture', -21, ARRAY['slika3.jpg'], NOW(), NOW());

INSERT INTO stakeholders."Users"(
	"Id", "Username", "Password", "Role", "IsActive")
	VALUES (-11, 'turista1', 'turista1', 2, TRUE);