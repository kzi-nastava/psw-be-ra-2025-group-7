-- Blog seed podaci za testove

-- Draft blog (za autora -11)
INSERT INTO blog."BlogPosts" ("Id", "AuthorId", "Title", "Description", "CreatedAt", "Status", "LastModifiedAt")
VALUES (-1, -11, 'Draft Blog Post', 'Ovo je blog u Draft statusu.', NOW(), 0, NULL);

-- Published blog (za autora -11) 
INSERT INTO blog."BlogPosts" ("Id", "AuthorId", "Title", "Description", "CreatedAt", "Status", "LastModifiedAt")
VALUES (-2, -11, 'Published Blog Post', 'Ovo je objavljen blog.', NOW(), 1, NULL);

-- Jos jedan Published blog (za drugog autora -12) 
INSERT INTO blog."BlogPosts" ("Id", "AuthorId", "Title", "Description", "CreatedAt", "Status", "LastModifiedAt")
VALUES (-3, -12, 'Another Published Blog', 'Jos jedan objavljen blog od drugog autora.', NOW(), 1, NULL);

INSERT INTO blog."BlogVote" ("Id", "UserId", "Value", "VotedAt", "BlogPostsId")
VALUES
    (-1, -11, 1, NOW(), -1),
    (-2, -12, -1, NOW(), -1);
