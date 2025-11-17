-- Seed za test Create() - turist -1
INSERT INTO tours."TourProblems" 
("Id", "TourId", "TouristId", "Category", "Priority", "Description", "TimeReported")
VALUES 
(-101, 1, -1, 'Safety', 'Low', 'Klisavo pri spustanju posle 2. kontrolne tacke', '2024-01-01T12:00:00Z'),
(-102, 1, -1, 'Navigation', 'Medium', 'Skretanje nije jasno obelezeno kod stare cesme', '2024-02-01T12:00:00Z');

-- Seed za test Create() - turist -2
INSERT INTO tours."TourProblems" 
("Id", "TourId", "TouristId", "Category", "Priority", "Description", "TimeReported")
VALUES 
(-201, 2, -2, 'Equipment', 'High', 'Merdevine su ostecene', '2024-03-01T12:00:00Z');

-- Seed za test Updates() - turist -1
INSERT INTO tours."TourProblems" 
("Id", "TourId", "TouristId", "Category", "Priority", "Description", "TimeReported")
VALUES 
(-1, 1, -1, 'Equipment', 'Medium', 'Initial equipment issue to be updated.', '2024-03-01T12:00:00Z');

-- Seed za test Deletes() - turist -1
INSERT INTO tours."TourProblems" 
("Id", "TourId", "TouristId", "Category", "Priority", "Description", "TimeReported")
VALUES 
(-3, 2, -1, 'Safety', 'Low', 'Safety related problem to be deleted.', '2024-03-01T12:00:00Z');