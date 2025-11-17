INSERT INTO tours."Tours" ("Id", "Name", "Description", "Difficulty", "Tags", "Status", "Price", "AuthorId")
VALUES 
    (1, 'Fruška Gora Highlights', 'Obilazak najpoznatijih manastira i vidikovaca Fruške Gore', 1, '{priroda,manastiri,kultura}', 0, 0, 1),
    (2, 'Tara National Park Trek', 'Dvodnevna tura kroz netaknutu prirodu nacionalnog parka Tara', 2, '{planina,kamping,priroda}', 0, 0, 1),
    (3, 'Beograd Walking Tour', 'Obilazak glavnih znamenitosti Beograda sa lokalnim vodičem', 0, '{grad,kultura,istorija}', 1, 1500, 1),
    (4, 'Đavolja Varoš Mystery', 'Poseta čudesnim kamenim formacijama i istraživanje lokalnih legendi', 1, '{priroda,geologija,folklore}', 1, 2500, 2),
    (5, 'Stari Ras and Sopoćani', 'UNESCO svetska baština - srednjovekovni manastiri i tvrđave', 1, '{unesco,istorija,manastiri}', 2, 3000, 2);

SELECT * FROM tours."Tours";