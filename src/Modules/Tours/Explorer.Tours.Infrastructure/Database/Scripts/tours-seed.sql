INSERT INTO tours."Tours" ("Id", "Name", "Description", "Difficulty", "Tags", "Status", "Price", "AuthorId")
VALUES 
    (-1, 'Obilazak Petrovaradinske tvrđave', 'Detaljan obilazak istorijske tvrđave sa vodičem. Uključuje posetu muzeju i pogled sa satnog tornja.', 1, 'istorija,kultura,tvrđava', 1, 1500, -11),
    (-2, 'Fruška gora - planinarski izlet', 'Šestočasovni planinarski izlet kroz nacionalni park. Srednji nivo težine.', 2, 'priroda,planinarenje,fruška gora', 1, 2500, -12),
    (-3, 'Dunav - vožnja brodom', 'Romantična večernja vožnja Dunavom sa uključenom večerom.', 0, 'romantika,reka,brod', 1, 3500, -11),
    (-4, 'Stara Pazova - vinski turizam', 'Poseta vinarijama u okolini Stare Pazove sa degustacijom vina.', 1, 'vino,degustacija,gastro', 0, 4000, -13),
    (-5, 'Bike tour Novi Sad', 'Biciklistička tura kroz centar grada i park Dunavski kej.', 1, 'bicikl,grad,sport', 1, 1000, -12),
    (-6, 'Sremski Karlovci - jednodnevni izlet', 'Obilazak baroknog grada sa posetom kapele mira i degustacijom bermetskog vina.', 0, 'kultura,istorija,gastro', 1, 2000, -13),
    (-7, 'Ekstremna planinska avantura', 'Trodnevno pešačenje sa kampovanjem. Napredni nivo.', 3, 'ekstrem,planina,kampovanje', 0, 8000, -12);

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
