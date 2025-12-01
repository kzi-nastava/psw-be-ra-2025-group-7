INSERT INTO tours."Tours" ("Id", "Name", "Description", "Difficulty", "Tags", "Status", "Price", "AuthorId", "PublishedAt", "ArchivedAt")
VALUES 
    (-1, 'Test Tour 1 - Draft', 'First test tour for automated testing', 1, 'test,nature,hiking', 0, 0, -1, NULL, NULL),
    (-2, 'Test Tour 2 - Draft', 'Second test tour for automated testing', 2, 'test,mountain', 0, 50, -1, NULL, NULL),
    (-3, 'Test Tour 3 - Published', 'Published test tour', 1, 'test', 1, 100, -1, '2024-01-15 10:00:00', NULL),
    (-4, 'Test Tour 4 - Draft for Archive Failure', 'Draft test tour for archive failure test', 1, 'test,failure', 0, 80, -1, NULL, NULL),
    (-5, 'Test Tour 5 - Published for Reactivate', 'Published test tour for reactivation', 2, 'test,reactivate', 1, 120, -1, '2024-01-25 16:00:00', NULL),
    (-6, 'Test Tour 6 - Draft for Reactivate Failure', 'Draft test tour for reactivate failure test', 1, 'test,failure', 0, 90, -1, NULL, NULL);