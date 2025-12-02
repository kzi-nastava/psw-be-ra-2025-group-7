INSERT INTO tours."KeyPoints" ("Id", "Latitude", "Longitude", "Name", "Description", "ImageUrl", "Secret", "TourId")
VALUES
    (-510, 45.2671, 19.8335, 'KP1 Tour 1', 'First key point for tour 1', NULL, 'secret1', -500),
    (-511, 45.2680, 19.8340, 'KP2 Tour 1', 'Second key point for tour 1', NULL, 'secret2', -500);

-- KeyPoints za Tour -501 (potrebno za publish failure test)
INSERT INTO tours."KeyPoints" ("Id", "Latitude", "Longitude", "Name", "Description", "ImageUrl", "Secret", "TourId")
VALUES
    (-512, 45.2675, 19.8350, 'KP1 Tour 2', 'First key point for tour 2', NULL, 'secret3', -501),
    (-513, 45.2685, 19.8360, 'KP2 Tour 2', 'Second key point for tour 2', NULL, 'secret4', -501);
