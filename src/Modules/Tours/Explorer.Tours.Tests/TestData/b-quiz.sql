INSERT INTO tours."Quiz"("Id", "AuthorId", "Title")
VALUES (1, 1, 'Kviz o Novom Sadu');
INSERT INTO tours."Question"("Id", "QuizId", "Content", "AllowsMultipleCorrect")
VALUES (2, 1, 'Koja tvrđava se nalazi u Novom Sadu?', FALSE);
INSERT INTO tours."Options"("Id", "QuestionId", "Text", "IsCorrect", "Feedback")
VALUES 
    (9, 2, 'Petrovaradinska tvrđava', TRUE, 'Tačno! Ovo je najpoznatija znamenitost Novog Sada.'),
    (8, 2, 'Niška tvrđava', FALSE, 'Netačno, ova tvrđava je u Nišu.'),
    (7, 2, 'Kalemegdan', FALSE, 'Netačno, Kalemegdan je u Beogradu.'),
    (6, 2, 'Smederevska tvrđava', FALSE, 'Netačno, ova tvrđava nije u Novom Sadu.');
INSERT INTO tours."Question"("Id", "QuizId", "Content", "AllowsMultipleCorrect")
VALUES (3, 1, 'Koje manifestacije se održavaju u Novom Sadu?', TRUE);
INSERT INTO tours."Options"("Id", "QuestionId", "Text", "IsCorrect", "Feedback")
VALUES 
    (11, 3, 'EXIT festival', TRUE, 'Tačno! EXIT je najveći muzički festival u regionu.'),
    (12, 3, 'Zmaj Jovina dečja manifestacija', TRUE, 'Tačno! Poznata manifestacija za decu u Novom Sadu.'),
    (13, 3, 'Guča trubački festival', FALSE, 'Ne, ovo se održava u Guči.'),
    (14, 3, 'Beer Fest', FALSE, 'Ne, Beer Fest se održava u Beogradu.');
