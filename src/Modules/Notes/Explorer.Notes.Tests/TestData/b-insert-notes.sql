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

-- Note za turistu -11 (Observation tip) - OVA CE BITI OBRISANA u Delete testu
INSERT INTO notes."Notes" ("Id", "UserId", "Title", "Content", "Type", "Tags", "TourId", "IsPinned", "CreatedAt", "UpdatedAt")
VALUES (-4, -11, 'Best coffee spots', 'Found amazing coffee at Kafeterija near the park', 3, '["Coffee", "Belgrade"]', NULL, false, '2024-12-18 16:00:00+00', '2024-12-18 16:00:00+00');

-- Note vezana za turu (tourist -1 ima purchased tour -3)
INSERT INTO notes."Notes" ("Id", "UserId", "Title", "Content", "Type", "Tags", "TourId", "IsPinned", "CreatedAt", "UpdatedAt")
VALUES (-5, -1, 'Dunav river cruise notes', 'Remember to bring camera for sunset photos', 0, '["Dunav", "Photography"]', -3, false, '2024-12-22 12:00:00+00', '2024-12-22 12:00:00+00');

-- Note za tags test - za testiranje GetUserTags
INSERT INTO notes."Notes" ("Id", "UserId", "Title", "Content", "Type", "Tags", "TourId", "IsPinned", "CreatedAt", "UpdatedAt")
VALUES (-6, -11, 'Tag test note 1', 'Content for tag test', 0, '["Kalemegdan", "Travel", "Belgrade"]', NULL, false, '2024-12-23 10:00:00+00', '2024-12-23 10:00:00+00');

INSERT INTO notes."Notes" ("Id", "UserId", "Title", "Content", "Type", "Tags", "TourId", "IsPinned", "CreatedAt", "UpdatedAt")
VALUES (-7, -11, 'Tag test note 2', 'Another note for tag test', 1, '["Coffee", "Food"]', NULL, false, '2024-12-23 11:00:00+00', '2024-12-23 11:00:00+00');

-- Note za search test - za testiranje pretrage
INSERT INTO notes."Notes" ("Id", "UserId", "Title", "Content", "Type", "Tags", "TourId", "IsPinned", "CreatedAt", "UpdatedAt")
VALUES (-8, -11, 'Coffee exploration', 'Exploring best coffee shops in town', 3, '["Coffee", "Exploration"]', NULL, false, '2024-12-24 10:00:00+00', '2024-12-24 10:00:00+00');

INSERT INTO notes."Notes" ("Id", "UserId", "Title", "Content", "Type", "Tags", "TourId", "IsPinned", "CreatedAt", "UpdatedAt")
VALUES (-9, -11, 'Tea places', 'Great tea selection downtown', 1, '["Tea", "Downtown"]', NULL, false, '2024-12-24 11:00:00+00', '2024-12-24 11:00:00+00');