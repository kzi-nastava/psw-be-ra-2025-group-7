-- UserProfiles za autore
INSERT INTO stakeholders."UserProfiles"(
    "Id", "UserId", "FirstName", "LastName", "ProfilePicture", "Biography", "Motto")
VALUES (-11, -11, 'Ana', 'Anić', 'https://example.com/ana.jpg', 
    'Entuzijasta turizma sa strašću za istraživanje novih destinacija.', 
    'Travel far, travel wide!');

INSERT INTO stakeholders."UserProfiles"(
    "Id", "UserId", "FirstName", "LastName", "ProfilePicture", "Biography", "Motto")
VALUES (-12, -12, 'Lena', 'Lenić', 'https://example.com/lena.jpg', 
    'Iskusni turistički vodič sa 5 godina iskustva u organizaciji tura.', 
    'Life is a journey, not a destination.');

INSERT INTO stakeholders."UserProfiles"(
    "Id", "UserId", "FirstName", "LastName", "ProfilePicture", "Biography", "Motto")
VALUES (-13, -13, 'Sara', 'Sarić', 'https://example.com/sara.jpg', 
    'Ljubitelj prirode i avanturista koji voli da deli svoja iskustva.', 
    'Adventure is out there!');

-- UserProfiles za turiste
INSERT INTO stakeholders."UserProfiles"(
    "Id", "UserId", "FirstName", "LastName", "ProfilePicture", "Biography", "Motto", "CurrentLatitude", "CurrentLongitude")
VALUES (-21, -21, 'Pera', 'Perić', 'https://example.com/pera.jpg', 
    'Student računarstva koji voli da putuje i upoznaje nove kulture.', 
    'Carpe diem!', 44.8176, 20.4633);

INSERT INTO stakeholders."UserProfiles"(
    "Id", "UserId", "FirstName", "LastName", "ProfilePicture", "Biography", "Motto")
VALUES (-22, -22, 'Mika', 'Mikić', 'https://example.com/mika.jpg', 
    'Turista koji voli arhitekturu i istoriju evropskih gradova.', 
    'Collect moments, not things.');

