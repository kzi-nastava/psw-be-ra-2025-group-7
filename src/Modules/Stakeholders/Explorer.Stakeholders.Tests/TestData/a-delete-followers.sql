-- Clean existing test data
DELETE FROM stakeholders."Followers" WHERE "Id" < 0;
DELETE FROM stakeholders."FollowerMessages" WHERE "Id" < 0;
DELETE FROM stakeholders."ClubMessages" WHERE "Id" < 0;
DELETE FROM stakeholders."Notifications" WHERE "Id" < 0;
