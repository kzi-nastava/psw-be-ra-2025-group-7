-- Test notes za integraciju

-- Note za turistu -11 (Plan tip)
INSERT INTO notes."Notes" ("Id", "UserId", "Title", "Content", "Type", "Tags", "TourId", "IsPinned", "CreatedAt", "UpdatedAt")
VALUES (-1, -11, 'Weekend trip to Kalemegdan', 'Visit fortress, take photos, try local food', 0, '["Kalemegdan", "Weekend", "Food"]', NULL, false, '2024-12-20 10:00:00+00', '2024-12-20 10:00:00+00');

-- Note za turistu -11 (Idea tip, pinned)
INSERT INTO notes."Notes" ("Id", "UserId", "Title", "Content", "Type", "Tags", "TourId", "IsPinned", "CreatedAt", "UpdatedAt")
VALUES (-2, -11, 'Travel bucket list', 'Countries to visit: Japan, Iceland, New Zealand', 1, '["Travel", "Ideas"]', NULL, true, '2024-12-19 14:00:00+00', '2024-12-21 09:00:00+00');

-- Note za turistu -12 (Reminder tip)
INSERT INTO notes."Notes" ("Id", "UserId", "Title", "Content", "Type", "Tags", "TourId", "IsPinned", "CreatedAt", "UpdatedAt")
VALUES (-3, -12, 'Pack hiking boots', 'Don''t forget waterproof hiking boots for mountain tour', 2, '["Packing", "Hiking"]', NULL, false, '2024-12-21 08:00:00+00', '2024-12-21 08:00:00+00');

-- Note za turistu -11 (Observation tip)
INSERT INTO notes."Notes" ("Id", "UserId", "Title", "Content", "Type", "Tags", "TourId", "IsPinned", "CreatedAt", "UpdatedAt")
VALUES (-4, -11, 'Best coffee spots', 'Found amazing coffee at Kafeterija near the park', 3, '["Coffee", "Belgrade"]', NULL, false, '2024-12-18 16:00:00+00', '2024-12-18 16:00:00+00');