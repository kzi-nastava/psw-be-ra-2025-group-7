-- Koristi Tour -500 i -501 koji sigurno postoje (iz c-insert-tour-keypoints.sql)
INSERT INTO tours."PublicPointRequests" 
("Id", "TourId", "KeyPointIndex", "AuthorId", "Status", "AdminComment", "CreatedAt", "ProcessedAt")
VALUES
-- Pending
(-101, -500, 0, 1, 0, NULL, '2025-01-08 10:00:00+00', NULL),
(-102, -500, 1, 1, 0, NULL, '2025-01-09 11:30:00+00', NULL),
(-103, -501, 0, 2, 0, NULL, '2025-01-10 09:15:00+00', NULL),

-- Approved
(-104, -500, 0, 1, 1, 'Odlična tačka!', '2025-01-05 08:00:00+00', '2025-01-06 14:30:00+00'),
(-105, -501, 1, 2, 1, NULL, '2025-01-04 12:00:00+00', '2025-01-05 16:00:00+00'),

-- Rejected
(-106, -501, 0, 1, 2, 'Ne valja.', '2025-01-03 10:00:00+00', '2025-01-04 11:00:00+00'),
(-107, -500, 1, 2, 2, 'Loše.', '2025-01-02 14:00:00+00', '2025-01-03 09:30:00+00');