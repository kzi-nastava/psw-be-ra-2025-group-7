-- Test data for TourExecutions
INSERT INTO tours."TourExecutions" ("Id", "TouristId", "TourId", "StartedAt", "CompletedAt", "AbandonedAt", "Status", "StartLatitude", "StartLongitude", "LastActivity", "UnlockedKeyPointIndices", "KeyPointUnlockTimes")
VALUES 
    -- For CompleteTour_Updates_Status_To_Completed test
    (-1, -1, -3, NOW() - INTERVAL '1 hour', NULL, NULL, 0, 45.2551, 19.8636, NOW() - INTERVAL '30 minutes', '[0, 1, 2]'::jsonb, '{{"0": "2024-01-15T10:35:00Z", "1": "2024-01-15T10:50:00Z", "2": "2024-01-15T11:00:00Z"}}'::jsonb),
    
    -- For AbandonTour_Updates_Status_To_Abandoned test
    (-2, -1, -3, NOW() - INTERVAL '1 hour', NULL, NULL, 0, 45.2551, 19.8636, NOW() - INTERVAL '30 minutes', '[0, 1]'::jsonb, '{{"0": "2024-01-15T10:35:00Z", "1": "2024-01-15T10:50:00Z"}}'::jsonb),
    
    -- For CheckKeyPointProximity_Unlocks_KeyPoint_When_Near test (tourist -1)
    (-3, -1, -3, NOW() - INTERVAL '1 hour', NULL, NULL, 0, 45.2551, 19.8636, NOW() - INTERVAL '2 minutes', '[0, 1]'::jsonb, '{{"0": "2024-01-15T10:35:00Z", "1": "2024-01-15T10:50:00Z"}}'::jsonb),
    
    -- For CheckKeyPointProximity_Updates_LastActivity_Even_When_Not_Near test (tourist -1)
    (-4, -1, -3, NOW() - INTERVAL '1 hour', NULL, NULL, 0, 45.2551, 19.8636, NOW() - INTERVAL '2 minutes', '[0, 1]'::jsonb, '{{"0": "2024-01-15T10:35:00Z", "1": "2024-01-15T10:50:00Z"}}'::jsonb),
    
    -- For UpdateLastActivity_Updates_Timestamp test (tourist -1)
    (-5, -1, -3, NOW() - INTERVAL '1 hour', NULL, NULL, 0, 45.2551, 19.8636, NOW() - INTERVAL '2 minutes', '[0, 1]'::jsonb, '{{"0": "2024-01-15T10:35:00Z", "1": "2024-01-15T10:50:00Z"}}'::jsonb),
    
    -- For GetProgressPercentage_Returns_Correct_Percentage test (tourist -1, 1 out of 3 keypoints = 33.3%)
    (-6, -1, -3, NOW() - INTERVAL '1 hour', NULL, NULL, 0, 45.2551, 19.8636, NOW() - INTERVAL '30 minutes', '[0]'::jsonb, '{{"0": "2024-01-15T10:35:00Z"}}'::jsonb),
    
    -- For GetActiveExecution_Returns_Active_Execution_For_Tour test (tourist -1, active execution)
    (-7, -1, -3, NOW() - INTERVAL '1 hour', NULL, NULL, 0, 45.2551, 19.8636, NOW() - INTERVAL '30 minutes', '[0, 1]'::jsonb, '{{"0": "2024-01-15T10:35:00Z", "1": "2024-01-15T10:50:00Z"}}'::jsonb),
    
    -- For GetKeyPointSecret_Returns_Secret_When_KeyPoint_Unlocked test (tourist -1, keypoint 0 unlocked)
    (-8, -1, -3, NOW() - INTERVAL '1 hour', NULL, NULL, 0, 45.2551, 19.8636, NOW() - INTERVAL '30 minutes', '[0, 1]'::jsonb, '{{"0": "2024-01-15T10:35:00Z", "1": "2024-01-15T10:50:00Z"}}'::jsonb),
    
    -- For GetKeyPointSecret_Throws_When_KeyPoint_Not_Unlocked test (tourist -1, only keypoint 0 unlocked, NOT 1)
    (-9, -1, -3, NOW() - INTERVAL '1 hour', NULL, NULL, 0, 45.2551, 19.8636, NOW() - INTERVAL '30 minutes', '[0]'::jsonb, '{{"0": "2024-01-15T10:35:00Z"}}'::jsonb),
    
    -- For GetExecutionHistory tests - completed execution (tourist -1)
    (-10, -1, -3, '2024-01-10 09:00:00', '2024-01-10 15:30:00', NULL, 1, 45.2551, 19.8636, '2024-01-10 15:30:00', '[0, 1, 2]'::jsonb, '{{"0": "2024-01-10T09:15:00Z", "1": "2024-01-10T11:30:00Z", "2": "2024-01-10T14:00:00Z"}}'::jsonb),
    
    -- For GetExecutionHistory tests - abandoned execution (tourist -2)
    (-11, -2, -3, '2024-01-12 10:00:00', NULL, '2024-01-12 12:00:00', 2, 45.2551, 19.8636, '2024-01-12 12:00:00', '[0]'::jsonb, '{{"0": "2024-01-12T10:20:00Z"}}'::jsonb),
    
    -- For TourReview tests - Active execution for tourist -2 with 2 of 3 keypoints (66.7% > 35%)
    (-12, -2, -3, NOW() - INTERVAL '4 hours', NULL, NULL, 0, 45.2551, 19.8636, NOW() - INTERVAL '30 minutes', '[0, 1]'::jsonb, '{{"0": "2024-01-18T08:20:00Z", "1": "2024-01-18T10:00:00Z"}}'::jsonb),
    
    -- For TourReview CreateReview test - Tourist -2 execution with > 35% progress
    (-13, -2, -3, NOW() - INTERVAL '4 hours', NULL, NULL, 0, 45.2551, 19.8636, NOW() - INTERVAL '30 minutes', '[0, 1]'::jsonb, '{{"0": "2024-01-19T09:15:00Z", "1": "2024-01-19T11:30:00Z"}}'::jsonb),
    
    -- For TourReview tests - Tourist -2 execution with only 1 of 3 keypoints (33.3% < 35%)
    (-14, -2, -3, NOW() - INTERVAL '1 hour', NULL, NULL, 0, 45.2551, 19.8636, NOW() - INTERVAL '30 minutes', '[0]'::jsonb, '{{"0": "2024-01-20T10:15:00Z"}}'::jsonb);
