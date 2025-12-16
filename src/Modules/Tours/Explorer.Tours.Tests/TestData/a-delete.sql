-- Delete from tables that exist - use DO block to handle missing tables gracefully
DO $$ 
BEGIN
    -- Child tables first (FK dependencies)
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'tours' AND table_name = 'OrderItems') THEN
        DELETE FROM tours."OrderItems";
    END IF;
    
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'tours' AND table_name = 'ShoppingCarts') THEN
        DELETE FROM tours."ShoppingCarts";
    END IF;

    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'tours' AND table_name = 'TourPurchaseTokens') THEN
        DELETE FROM tours."TourPurchaseTokens";
    END IF;
    
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'tours' AND table_name = 'TourDurations') THEN
        DELETE FROM tours."TourDurations";
    END IF;
    
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'tours' AND table_name = 'KeyPoints') THEN
        DELETE FROM tours."KeyPoints";
    END IF;
    
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'tours' AND table_name = 'Tours') THEN
        DELETE FROM tours."Tours";
    END IF;

    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'tours' AND table_name = 'Equipment') THEN
        DELETE FROM tours."Equipment";
    END IF;
    
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'tours' AND table_name = 'TourProblems') THEN
        DELETE FROM tours."TourProblems";
    END IF;
    
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'tours' AND table_name = 'TourJournals') THEN
        DELETE FROM tours."TourJournals";
    END IF;
    
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'tours' AND table_name = 'Facility') THEN
        DELETE FROM tours."Facility";
    END IF;
    
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'tours' AND table_name = 'Monuments') THEN
        DELETE FROM tours."Monuments";
    END IF;
    
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'tours' AND table_name = 'Notifications') THEN
        DELETE FROM tours."Notifications";
    END IF;
    
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'tours' AND table_name = 'PublicPointRequests') THEN
        DELETE FROM tours."PublicPointRequests";
    END IF;

    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'tours' AND table_name = 'Option') THEN
        DELETE FROM tours."Option";
    END IF;
    
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'tours' AND table_name = 'Questions') THEN
        DELETE FROM tours."Questions";
    END IF;
    
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'tours' AND table_name = 'Quizzes') THEN
        DELETE FROM tours."Quizzes";
    END IF;

    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'tours' AND table_name = 'TouristEquipment') THEN
        DELETE FROM tours."TouristEquipment";
    END IF;
    
    IF EXISTS (SELECT FROM information_schema.tables WHERE table_schema = 'tours' AND table_name = 'AnnualAwards') THEN
        DELETE FROM tours."AnnualAwards";
    END IF;
END $$;