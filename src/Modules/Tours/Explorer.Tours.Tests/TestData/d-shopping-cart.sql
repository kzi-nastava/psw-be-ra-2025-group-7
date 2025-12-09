-- ============================================================
-- d-shopping-cart.sql
-- Testni podaci za ShoppingCart funkcionalnost
-- ============================================================

-- ------------------------------------------------------------
-- 1. Dodatne Published ture za testiranje Shopping Cart-a
-- ------------------------------------------------------------
-- Tura -3 ve? postoji kao Published (Price: 100)
-- Tura -5 ve? postoji kao Published (Price: 120)

-- Dodajemo još jednu Published turu sa razli?itom cenom
INSERT INTO tours."Tours" ("Id", "Name", "Description", "Difficulty", "Tags", "Status", "Price", "AuthorId", "PublishedAt", "ArchivedAt")
VALUES 
    (-10, 'Test Tour Published - For Cart', 'Published tour specifically for shopping cart tests', 1, 'test,shopping', 1, 75.50, -1, '2024-01-20 12:00:00', NULL);

-- Key points za turu -10 (potrebno je minimum 2 za Published status)
INSERT INTO tours."KeyPoints" ("Id", "Latitude", "Longitude", "Name", "Description", "ImageUrl", "Secret", "TourId")
VALUES
    (-520, 45.2600, 19.8300, 'KP1 Tour -10', 'First key point for tour -10', NULL, 'secret10', -10),
    (-521, 45.2610, 19.8310, 'KP2 Tour -10', 'Second key point for tour -10', NULL, 'secret11', -10);

-- ------------------------------------------------------------
-- 2. Arhivirana tura za testiranje validacije
-- ------------------------------------------------------------
INSERT INTO tours."Tours" ("Id", "Name", "Description", "Difficulty", "Tags", "Status", "Price", "AuthorId", "PublishedAt", "ArchivedAt")
VALUES 
    (-100, 'Test Tour Archived', 'Archived tour - cannot be purchased', 1, 'test,archived', 2, 75.00, -1, '2024-01-10 10:00:00', '2024-02-01 10:00:00');

-- Key points za arhivirana turu -100
INSERT INTO tours."KeyPoints" ("Id", "Latitude", "Longitude", "Name", "Description", "ImageUrl", "Secret", "TourId")
VALUES
    (-530, 45.2620, 19.8320, 'KP1 Archived Tour', 'First key point for archived tour', NULL, 'secret_archived1', -100),
    (-531, 45.2630, 19.8330, 'KP2 Archived Tour', 'Second key point for archived tour', NULL, 'secret_archived2', -100);

-- ------------------------------------------------------------
-- 3. Draft tura za testiranje validacije
-- ------------------------------------------------------------
INSERT INTO tours."Tours" ("Id", "Name", "Description", "Difficulty", "Tags", "Status", "Price", "AuthorId", "PublishedAt", "ArchivedAt")
VALUES 
    (-101, 'Test Tour Draft', 'Draft tour - cannot be purchased', 1, 'test,draft', 0, 80.00, -1, NULL, NULL);

-- Key points za draft turu -101 (draft ture mogu imati key points, samo nisu objavljene)
INSERT INTO tours."KeyPoints" ("Id", "Latitude", "Longitude", "Name", "Description", "ImageUrl", "Secret", "TourId")
VALUES
    (-540, 45.2640, 19.8340, 'KP1 Draft Tour', 'First key point for draft tour', NULL, 'secret_draft1', -101),
    (-541, 45.2650, 19.8350, 'KP2 Draft Tour', 'Second key point for draft tour', NULL, 'secret_draft2', -101);

-- ------------------------------------------------------------
-- 4. Shopping Carts za testiranje
-- ------------------------------------------------------------
-- Shopping cart za turistu ID=1 sa stavkama (za testiranje GetMyCart)
INSERT INTO tours."ShoppingCarts" ("Id", "TouristId", "TotalPrice")
VALUES 
    (-1, 1, 220.00);

-- Stavke u korpi za turistu 1
INSERT INTO tours."OrderItems" ("Id", "TourId", "TourName", "Price", "ShoppingCartId")
VALUES 
    (-1, -3, 'Test Tour 3 - Published', 100.00, -1),
    (-2, -5, 'Test Tour 5 - Published for Reactivate', 120.00, -1);

-- ------------------------------------------------------------
-- 5. Dodatne Published ture sa cenom 0 za testiranje
-- ------------------------------------------------------------
-- Za testiranje da korpa radi ispravno sa besplatnim turama
INSERT INTO tours."Tours" ("Id", "Name", "Description", "Difficulty", "Tags", "Status", "Price", "AuthorId", "PublishedAt", "ArchivedAt")
VALUES 
    (-11, 'Free Tour Published', 'Free published tour for testing', 1, 'test,free', 1, 0, -1, '2024-01-22 14:00:00', NULL);

-- Key points za besplatnu turu -11
INSERT INTO tours."KeyPoints" ("Id", "Latitude", "Longitude", "Name", "Description", "ImageUrl", "Secret", "TourId")
VALUES
    (-550, 45.2660, 19.8360, 'KP1 Free Tour', 'First key point for free tour', NULL, 'secret_free1', -11),
    (-551, 45.2670, 19.8370, 'KP2 Free Tour', 'Second key point for free tour', NULL, 'secret_free2', -11);

-- ------------------------------------------------------------
-- NAPOMENA ZA TESTOVE:
-- ------------------------------------------------------------
-- Testovi koriste slede?e TouristId-jeve: 1, 2, 3, 4, 5, 6, 7
-- Published ture dostupne za dodavanje u korpu: -3, -5, -10, -11
-- Arhivirana tura (ne može u korpu): -100
-- Draft tura (ne može u korpu): -101
-- ------------------------------------------------------------
