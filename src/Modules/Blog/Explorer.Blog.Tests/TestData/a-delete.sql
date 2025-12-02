-- Briši sve BlogComments PRVO (zbog foreign key-a)
DELETE FROM blog."BlogComments";

-- Briši sve BlogPostImages (owned entity)
DELETE FROM blog."BlogPostImages";

-- Briši Tours Equipment (ako je potrebno)
DELETE FROM tours."Equipment";

DELETE FROM blog."BlogVote";

-- Briši sve BlogPosts
DELETE FROM blog."BlogPosts";
