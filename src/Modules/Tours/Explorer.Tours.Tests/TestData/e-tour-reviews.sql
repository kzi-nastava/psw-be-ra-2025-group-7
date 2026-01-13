-- Test data for TourReviews
INSERT INTO tours."TourReviews" ("Id", "TouristId", "TourId", "TourExecutionId", "Rating", "Comment", "CreatedAt", "UpdatedAt", "TourProgressPercentage", "ImageUrls")
VALUES 
    -- Review from tourist -1 for tour -3 (completed execution -10)
    (-1, -1, -3, -10, 5, 'Excellent tour! I loved every moment of it. The key points were well planned and the route was beautiful.', '2024-01-10 16:00:00', NULL, 100.0, '["https://example.com/tour-photo1.jpg", "https://example.com/tour-photo2.jpg"]'::jsonb);
    
    -- Review from tourist -2 for tour -3 (active execution -12 with 66.7% progress > 35%)
    --(-2, -2, -3, -12, 4, 'Great tour so far! Really enjoying the experience. The locations are amazing.', '2024-01-18 13:00:00', NULL, 66.7, '["https://example.com/tour-photo3.jpg"]'::jsonb);
