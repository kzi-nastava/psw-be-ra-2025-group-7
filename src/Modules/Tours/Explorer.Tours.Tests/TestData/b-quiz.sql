INSERT INTO tours."Quiz"("Id", "AuthorId", "Title")
VALUES (100, 1, 'Kviz o Novom Sadu');

INSERT INTO tours."Question"("Id", "QuizId", "Content", "AllowsMultipleCorrect")
VALUES (101, 100, 'Koja tvrđava se nalazi u Novom Sadu?', FALSE);

INSERT INTO tours."Options"("Id", "QuestionId", "Text", "IsCorrect", "Feedback")
VALUES 
    (1011, 101, 'Petrovaradinska tvrđava', TRUE, 'Tačno! Ovo je najpoznatija znamenitost Novog Sada.'),
    (1012, 101, 'Niška tvrđava', FALSE, 'Netačno, ova tvrđava je u Nišu.'),
    (1013, 101, 'Kalemegdan', FALSE, 'Netačno, Kalemegdan je u Beogradu.'),
    (1014, 101, 'Smederevska tvrđava', FALSE, 'Netačno, ova tvrđava nije u Novom Sadu.');
INSERT INTO tours."Question"("Id", "QuizId", "Content", "AllowsMultipleCorrect")
VALUES (102, 100, 'Koje manifestacije se održavaju u Novom Sadu?', TRUE);

INSERT INTO tours."Options"("Id", "QuestionId", "Text", "IsCorrect", "Feedback")
VALUES 
    (1021, 102, 'EXIT festival', TRUE, 'Tačno! EXIT je najveći muzički festival u regionu.'),
    (1022, 102, 'Zmaj Jovina dečja manifestacija', TRUE, 'Tačno! Poznata manifestacija za decu u Novom Sadu.'),
    (1023, 102, 'Guča trubački festival', FALSE, 'Ne, ovo se održava u Guči.'),
    (1024, 102, 'Beer Fest', FALSE, 'Ne, Beer Fest se održava u Beogradu.');
