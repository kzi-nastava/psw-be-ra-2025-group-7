INSERT INTO blog."BlogPosts"
    ("Id", "AuthorId", "Title", "Description", "CreatedAt", "Status", "LastModifiedAt")
VALUES
    (-1, -21, 'Test blog', 'Ovo je test blog post iz TestData skripte.', NOW(), 0, NULL);

INSERT INTO blog."BlogVote" ("Id", "UserId", "Value", "VotedAt", "BlogPostsId")
VALUES
    (-1, -21, 1, NOW(), -1),
    (-2, -22, -1, NOW(), -1);