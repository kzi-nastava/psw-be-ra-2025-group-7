-- ============================================================
-- d-tour-requests.sql
-- Test data za TourRequest i TourRequestResponse funkcionalnost
-- ============================================================

-- ------------------------------------------------------------
-- 1. TourRequests - različiti statusi i scenariji
-- ------------------------------------------------------------

-- Open request sa lokacijom (Tourist -21)
INSERT INTO tours."TourRequests" (
    "Id", "TouristId", "Title", "Description", 
    "Latitude", "Longitude", "Radius",
    "Budget", "PreferredDifficulty", "NumberOfParticipants", "PreferredDate",
    "Status", "CreatedAt", "ExpiresAt"
)
VALUES (
    -1, -21, 
    'Planinarska tura u okolini Beograda', 
    'Tražim iskusnog vodiča za dvodnevnu planinarsku turu u okolini Beograda. Interesuje me tura srednje težine sa prenoćištem u planinarskom domu.',
    44.8178, 20.4568, 50,
    15000, 1, 4, '2024-06-15 10:00:00',
    0, '2024-01-15 10:00:00', '2024-02-14 10:00:00'
);

-- Open request bez lokacije (Tourist -21)
INSERT INTO tours."TourRequests" (
    "Id", "TouristId", "Title", "Description", 
    "Latitude", "Longitude", "Radius",
    "Budget", "PreferredDifficulty", "NumberOfParticipants", "PreferredDate",
    "Status", "CreatedAt", "ExpiresAt"
)
VALUES (
    -2, -21,
    'Kulturna tura kroz Srbiju',
    'Interesuje me sedmodnevna kulturna tura koja obuhvata manastire i istorijske znamenitosti Srbije. Preferiram grupu do 10 ljudi.',
    NULL, NULL, NULL,
    25000, NULL, 2, NULL,
    0, '2024-01-20 14:00:00', '2024-02-19 14:00:00'
);

-- Open request sa odgovorima (Tourist -22) - PROMENJEN STATUS SA 1 NA 0
INSERT INTO tours."TourRequests" (
    "Id", "TouristId", "Title", "Description", 
    "Latitude", "Longitude", "Radius",
    "Budget", "PreferredDifficulty", "NumberOfParticipants", "PreferredDate",
    "Status", "CreatedAt", "ExpiresAt"
)
VALUES (
    -3, -22,
    'Vikend u prirodi - Fruška Gora',
    'Potreban mi je vodič za vikend aranžman na Fruškoj Gori. Interesuju me šetnje prirodom i razgledanje manastira.',
    45.1667, 19.8333, 20,
    8000, 0, 3, '2024-05-10 09:00:00',
    0, '2024-01-10 12:00:00', '2024-02-09 12:00:00'
);

-- Fulfilled request (Tourist -22)
INSERT INTO tours."TourRequests" (
    "Id", "TouristId", "Title", "Description", 
    "Latitude", "Longitude", "Radius",
    "Budget", "PreferredDifficulty", "NumberOfParticipants", "PreferredDate",
    "Status", "CreatedAt", "ExpiresAt"
)
VALUES (
    -4, -22,
    'Jednodnevna obilazak Novog Sada',
    'Potreban mi je vodič za jednodnevni obilazak Novog Sada i Petrovaradinske tvrđave.',
    45.2551, 19.8636, 10,
    5000, 0, 2, '2024-04-20 10:00:00',
    2, '2024-01-05 15:00:00', '2024-02-04 15:00:00'
);

-- Closed request (Tourist -21)
INSERT INTO tours."TourRequests" (
    "Id", "TouristId", "Title", "Description", 
    "Latitude", "Longitude", "Radius",
    "Budget", "PreferredDifficulty", "NumberOfParticipants", "PreferredDate",
    "Status", "CreatedAt", "ExpiresAt"
)
VALUES (
    -5, -21,
    'Zimska tura na Kopaoniku',
    'Tražim vodiča za zimsku turu na Kopaoniku sa skijaške staze.',
    43.2906, 20.8122, 15,
    20000, 2, 5, '2024-02-01 08:00:00',
    3, '2024-01-01 09:00:00', '2024-01-31 09:00:00'
);

-- Request koji ističe za 3 dana (za testiranje warning badge)
INSERT INTO tours."TourRequests" (
    "Id", "TouristId", "Title", "Description", 
    "Latitude", "Longitude", "Radius",
    "Budget", "PreferredDifficulty", "NumberOfParticipants", "PreferredDate",
    "Status", "CreatedAt", "ExpiresAt"
)
VALUES (
    -6, -21,
    'Ekspres tura - hitno',
    'Hitno tražim vodiča za kratku turu u okolini Beograda.',
    44.8178, 20.4568, 30,
    7000, 0, 2, '2024-03-05 10:00:00',
    0, 
    CURRENT_TIMESTAMP - CAST('27 days' AS INTERVAL), 
    CURRENT_TIMESTAMP + CAST('3 days' AS INTERVAL)
);

-- ------------------------------------------------------------
-- 2. TourRequestResponses - različiti tipovi odgovora
-- ------------------------------------------------------------

-- Existing Tour response za request -1 (Pending)
INSERT INTO tours."TourRequestResponses" (
    "Id", "TourRequestId", "AuthorId", "ResponseType",
    "TourId", "ProposalDescription", 
    "ProposedPrice", "Message", "Status", "CreatedAt"
)
VALUES (
    -1, -1, -1, 0,
    -3, NULL,
    14500, 'Imam idealno turu za vas! Tura obuhvata sve što ste tražili.',
    0, '2024-01-16 11:00:00'
);

-- Custom Proposal response za request -1 (Pending)
INSERT INTO tours."TourRequestResponses" (
    "Id", "TourRequestId", "AuthorId", "ResponseType",
    "TourId", "ProposalDescription",
    "ProposedPrice", "Message", "Status", "CreatedAt"
)
VALUES (
    -2, -1, -1, 1,
    NULL, 'Mogu da organizujem potpuno prilagođenu turu prema vašim željama. Tura će uključivati obilaske planina Avala i Kosmaj sa prenoćištem u planinarskom domu. Program uključuje profesionalnog vodiča, smeštaj i dva obroka dnevno.',
    13000, 'Radujem se što mogu da vam pomognem!',
    0, '2024-01-17 14:30:00'
);

-- Response za request -2 (Pending)
INSERT INTO tours."TourRequestResponses" (
    "Id", "TourRequestId", "AuthorId", "ResponseType",
    "TourId", "ProposalDescription",
    "ProposedPrice", "Message", "Status", "CreatedAt"
)
VALUES (
    -3, -2, -1, 1,
    NULL, 'Specijalizovan sam za kulturne ture kroz Srbiju. Moj program obuhvata posete manastirima Žiča, Studenica, Sopočani, Gradac i Djurdjevi Stupovi, kao i tvrđave Magliča. Tura traje 7 dana sa smeštajem u hotelima 3* i uključuje sve obroke.',
    24000, 'Imam iskustvo od 10 godina u vodjenju kulturnih tura.',
    0, '2024-01-21 09:15:00'
);

-- Responses za request -3 (oba Pending - test će jedan da prihvati)
INSERT INTO tours."TourRequestResponses" (
    "Id", "TourRequestId", "AuthorId", "ResponseType",
    "TourId", "ProposalDescription",
    "ProposedPrice", "Message", "Status", "CreatedAt"
)
VALUES (
    -4, -3, -1, 0,
    -5, NULL,
    7500, 'Imam postojeću turu koja savršeno odgovara vašim potrebama!',
    0, '2024-01-11 10:00:00'
);

INSERT INTO tours."TourRequestResponses" (
    "Id", "TourRequestId", "AuthorId", "ResponseType",
    "TourId", "ProposalDescription",
    "ProposedPrice", "Message", "Status", "CreatedAt"
)
VALUES (
    -5, -3, -1, 1,
    NULL, 'Vikend aranžman na Fruškoj Gori sa posetom manastirima Krušedol, Novo Hopovo i Vrdnik. Uključuje vodiča, prevoz i ručak.',
    8200, NULL,
    0, '2024-01-12 15:00:00'
);

-- Accepted response za request -4 (Fulfilled)
INSERT INTO tours."TourRequestResponses" (
    "Id", "TourRequestId", "AuthorId", "ResponseType",
    "TourId", "ProposalDescription",
    "ProposedPrice", "Message", "Status", "CreatedAt"
)
VALUES (
    -6, -4, -1, 0,
    -3, NULL,
    4800, 'Perfektna tura za vas!',
    1, '2024-01-06 11:30:00'
);

-- Rejected response za request -4
INSERT INTO tours."TourRequestResponses" (
    "Id", "TourRequestId", "AuthorId", "ResponseType",
    "TourId", "ProposalDescription",
    "ProposedPrice", "Message", "Status", "CreatedAt"
)
VALUES (
    -7, -4, -1, 1,
    NULL, 'Mogu da organizujem privatnu turu po Novom Sadu sa detaljnim istorijskim objašnjenjima.',
    5500, 'Imam licencu turističkog vodiča.',
    2, '2024-01-06 09:00:00'
);