INSERT INTO tours."Tours" ("Id", "Name", "Description", "Difficulty", "Tags", "Status", "Price", "AuthorId")
VALUES 
    (1, 'Fruška Gora Highlights', 'Obilazak najpoznatijih manastira i vidikovaca Fruške Gore', 1, '{priroda,manastiri,kultura}', 0, 0, 1),
    (2, 'Tara National Park Trek', 'Dvodnevna tura kroz netaknutu prirodu nacionalnog parka Tara', 2, '{planina,kamping,priroda}', 0, 0, 1),
    (3, 'Beograd Walking Tour', 'Obilazak glavnih znamenitosti Beograda sa lokalnim vodičem', 0, '{grad,kultura,istorija}', 1, 1500, 1),
    (4, 'Đavolja Varoš Mystery', 'Poseta čudesnim kamenim formacijama i istraživanje lokalnih legendi', 1, '{priroda,geologija,folklore}', 1, 2500, 2),
    (5, 'Stari Ras and Sopoćani', 'UNESCO svetska baština - srednjovekovni manastiri i tvrđave', 1, '{unesco,istorija,manastiri}', 2, 3000, 2);

-- Quizzes
INSERT INTO tours."Quizzes"("Id", "AuthorId", "Title")
VALUES (1, 1, 'Kviz o Novom Sadu');

-- Questions
INSERT INTO tours."Questions"("Id", "QuizId", "Content", "AllowsMultipleCorrect")
VALUES 
    (1, 1, 'Koja tvrđava se nalazi u Novom Sadu?', FALSE),
    (2, 1, 'Koje manifestacije se održavaju u Novom Sadu?', TRUE);

-- Options
INSERT INTO tours."Option"("Id", "QuestionId", "Text", "IsCorrect", "Feedback")
VALUES 
    (1, 1, 'Petrovaradinska tvrđava', TRUE, 'Tačno! Ovo je najpoznatija znamenitost Novog Sada.'),
    (2, 1, 'Niška tvrđava', FALSE, 'Netačno, ova tvrđava je u Nišu.'),
    (3, 2, 'EXIT festival', TRUE, 'Tačno! EXIT je najveći muzički festival u regionu.'),
    (4, 2, 'Beer Fest', FALSE, 'Ne, Beer Fest se održava u Beogradu.');

    
SELECT * FROM tours."Tours";
SELECT * FROM tours."Quizzes";
SELECT * FROM tours."Questions";
SELECT * FROM tours."Option";
