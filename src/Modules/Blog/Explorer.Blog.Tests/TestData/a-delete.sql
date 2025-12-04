-- Briši sve BlogComments PRVO (zbog foreign key-a)
DELETE FROM blog."BlogComments";

-- Briši sve BlogPostImages (owned entity)
DELETE FROM blog."BlogPostImages";

-- Briši sve BlogPosts
DELETE FROM blog."BlogVotes";
DELETE FROM blog."BlogPosts";
