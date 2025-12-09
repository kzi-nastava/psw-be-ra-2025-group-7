-- Koristimo negativne ID-jeve da izbegnemo konflikt sa auto-increment sekvencama
INSERT INTO tours."TourDurations" ("Id", "TransportType", "DurationInMinutes", "TourId")
VALUES
    (-100, '1', 50, -500);

INSERT INTO tours."TourDurations" ("Id", "TransportType", "DurationInMinutes", "TourId")
VALUES
    (-101, '1', 50, -2);