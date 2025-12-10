DELETE FROM tours."PublicPointRequests";
DELETE FROM tours."KeyPoints" WHERE "TourId" IN (-1, -2, -3, -10, -11, -12);
DELETE FROM tours."Tours" WHERE "Id" IN (-1, -2, -3, -10, -11, -12);
DELETE FROM tours."Equipment";
DELETE FROM tours."TourProblems";
DELETE FROM tours."TourJournals";
DELETE FROM tours."Facility";
DELETE FROM tours."Monuments";
DELETE FROM tours."Tours";

DELETE FROM tours."Quizzes"; 
DELETE FROM tours."Questions";
DELETE FROM tours."Option";

DELETE FROM tours."Equipment";
DELETE FROM tours."TouristEquipment";
DELETE FROM tours."AnnualAwards";