INSERT INTO stakeholders."Reviews"("Id", "Rating", "Comment", "PersonId", "CreatedAt", "UpdatedAt")
VALUES
    (-1, 5, 'Test review 1', -11, '2025-11-26 16:33:59+00', '2025-11-26 16:33:59+00'),
    (-2, 5, 'Test review 2', -12, '2025-11-26 16:33:59+00', '2025-11-26 16:33:59+00')
ON CONFLICT ("Id") DO NOTHING;
