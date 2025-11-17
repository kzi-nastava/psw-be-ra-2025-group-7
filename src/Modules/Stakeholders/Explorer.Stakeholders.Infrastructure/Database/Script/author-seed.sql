-- Seed data for Author users only

-- Insert Author Users
INSERT INTO stakeholders."Users"("Id", "Username", "Password", "Role", "IsActive")
VALUES (-11, 'autor1@gmail.com', 'autor1', 1, true);

INSERT INTO stakeholders."Users"("Id", "Username", "Password", "Role", "IsActive")
VALUES (-12, 'autor2@gmail.com', 'autor2', 1, true);

INSERT INTO stakeholders."Users"("Id", "Username", "Password", "Role", "IsActive")
VALUES (-13, 'autor3@gmail.com', 'autor3', 1, true);

-- Insert People for Authors
INSERT INTO stakeholders."People"("Id", "UserId", "Name", "Surname", "Email")
VALUES (-11, -11, 'Autor', 'Jedan', 'autor1@gmail.com');

INSERT INTO stakeholders."People"("Id", "UserId", "Name", "Surname", "Email")
VALUES (-12, -12, 'Autor', 'Dva', 'autor2@gmail.com');

INSERT INTO stakeholders."People"("Id", "UserId", "Name", "Surname", "Email")
VALUES (-13, -13, 'Autor', 'Tri', 'autor3@gmail.com');