-- Seed skripta za testne komentare
-- Dodaje komentare na postojeće blog postove

-- Komentari na blog post sa ID = -1 (Draft blog od autora -11)
-- NEĆE RADITI jer blog mora biti Published - ovo je namerno za testiranje validacije

-- Komentari na blog post sa ID = -2 (Published blog)
INSERT INTO blog."BlogComments" ("Id", "BlogPostId", "UserId", "Text", "CreatedAt", "LastModifiedAt")
VALUES 
    (-1, -2, -12, 'Odličan blog post! Hvala na korisnim informacijama.', '2024-11-20 10:00:00+00', NULL),
    (-2, -2, -13, 'Slažem se, veoma koristan sadržaj.', '2024-11-20 11:30:00+00', NULL),
    (-3, -2, -12, 'Imam pitanje u vezi treće tačke...', '2024-11-20 14:15:00+00', NULL),
    (-4, -2, -12, 'Može li neko da mi objasni detaljnije?', '2024-11-21 09:00:00+00', NULL),
    (-5, -2, -11, 'Sjajan primer! Pomoglo mi je mnogo.', '2024-11-21 16:45:00+00', NULL);

-- Komentari na blog post sa ID = -3 (ako postoji Published blog)
INSERT INTO blog."BlogComments" ("Id", "BlogPostId", "UserId", "Text", "CreatedAt", "LastModifiedAt")
VALUES 
    (-6, -3, -11, 'Interesantan pristup problemu!', '2024-11-22 08:00:00+00', NULL),
    (-7, -3, -12, 'Da li imate još primera?', '2024-11-22 10:30:00+00', NULL),
    (-8, -3, -12, 'Hvala što delite svoje iskustvo.', '2024-11-22 13:00:00+00', NULL);