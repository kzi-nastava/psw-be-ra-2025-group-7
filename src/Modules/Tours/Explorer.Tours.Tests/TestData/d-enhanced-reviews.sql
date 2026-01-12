INSERT INTO tours."EnhancedReviews"
("Id","TourId","TouristId","OverallRating","GuideQuality","ValueForMoney","RouteScenery","Difficulty","GroupSize","TextReview","CreatedAt")
VALUES
(-8001, -3, -1, 5, 5, 4, 5, 3, 4, 'seed review', NOW());

INSERT INTO tours."EnhancedReviewPros" ("Id","EnhancedReviewId","Text")
VALUES (-8101, -8001, 'Great guide');

INSERT INTO tours."EnhancedReviewCons" ("Id","EnhancedReviewId","Text")
VALUES (-8201, -8001, 'Too crowded');

INSERT INTO tours."EnhancedReviewTags" ("Id","EnhancedReviewId","Tag")
VALUES (-8301, -8001, 0);

INSERT INTO tours."EnhancedReviewImages" ("Id","EnhancedReviewId","Url","SizeBytes")
VALUES (-8401, -8001, '/enhanced-review-images/test1.jpg', 1000);

INSERT INTO tours."EnhancedReviewHelpfulVotes" ("Id","EnhancedReviewId","TouristId","VotedAt")
VALUES (-8501, -8001, -2, NOW());
