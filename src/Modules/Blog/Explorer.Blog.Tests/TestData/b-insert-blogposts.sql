INSERT INTO blog."BlogPosts" ("Id", "AuthorId", "Title", "Description", "CreatedAt")
VALUES
    (-1, -21, 'Test blog', 'Ovo je test blog post iz TestData skripte.', NOW());

INSERT INTO blog."BlogVote" ("Id", "UserId", "Value", "VotedAt", "BlogPostsId")
VALUES
    (-1, -21, 1, NOW(), -1),
    (-2, -22, -1, NOW(), -1);