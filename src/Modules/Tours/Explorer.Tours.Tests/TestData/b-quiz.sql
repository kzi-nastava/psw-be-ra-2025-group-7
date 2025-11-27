INSERT INTO tours."Quizzes"("Id", "AuthorId", "Title")
VALUES (21, 1, 'Kviz o Novom Sadu');

INSERT INTO tours."Questions"("Id", "QuizId", "Content", "AllowsMultipleCorrect")
VALUES (45, 21, 'Koja tvrđava se nalazi u Novom Sadu?', FALSE);

INSERT INTO tours."Option"("Id", "QuestionId", "Text", "IsCorrect", "Feedback")
VALUES 
    (69, 45, 'Petrovaradinska tvrđava', TRUE, 'Tačno! Ovo je najpoznatija znamenitost Novog Sada.'),
    (68, 45, 'Niška tvrđava', FALSE, 'Netačno, ova tvrđava je u Nišu.'),
    (67, 45, 'Kalemegdan', FALSE, 'Netačno, Kalemegdan je u Beogradu.'),
    (66, 45, 'Smederevska tvrđava', FALSE, 'Netačno, ova tvrđava nije u Novom Sadu.');

INSERT INTO tours."Questions"("Id", "QuizId", "Content", "AllowsMultipleCorrect")
VALUES (43, 21, 'Koje manifestacije se održavaju u Novom Sadu?', TRUE);

INSERT INTO tours."Option"("Id", "QuestionId", "Text", "IsCorrect", "Feedback")
VALUES 
    (111, 43, 'EXIT festival', TRUE, 'Tačno! EXIT je najveći muzički festival u regionu.'),
    (112, 43, 'Zmaj Jovina dečja manifestacija', TRUE, 'Tačno! Poznata manifestacija za decu u Novom Sadu.'),
    (113, 43, 'Guča trubački festival', FALSE, 'Ne, ovo se održava u Guči.'),
    (114, 43, 'Beer Fest', FALSE, 'Ne, Beer Fest se održava u Beogradu.');
