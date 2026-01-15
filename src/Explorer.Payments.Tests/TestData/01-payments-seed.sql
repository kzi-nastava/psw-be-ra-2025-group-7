-- ========================================
-- PAYMENTS MODULE TEST DATA
-- ========================================
-- This seed provides test data for Payments module integration tests
-- Execution order: This file runs first (01-*)
-- ========================================

-- ========================================
-- SCHEMA UPDATES - Add new columns/tables if they don't exist
-- ========================================
DO $$ 
BEGIN
    -- Add SolanaWalletAddress column to Wallets if it doesn't exist
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns 
        WHERE table_schema = 'payments' 
        AND table_name = 'Wallets' 
        AND column_name = 'SolanaWalletAddress'
    ) THEN
        ALTER TABLE payments."Wallets" 
        ADD COLUMN "SolanaWalletAddress" VARCHAR(50);
        
        CREATE INDEX "IX_Wallets_SolanaWalletAddress" 
        ON payments."Wallets" ("SolanaWalletAddress");
    END IF;
END $$;

-- Create CryptoDepositRequests table if it doesn't exist
CREATE TABLE IF NOT EXISTS payments."CryptoDepositRequests" (
    "Id" BIGSERIAL PRIMARY KEY,
    "UserId" BIGINT NOT NULL,
    "TransactionId" VARCHAR(100) NOT NULL,
    "CryptoAmount" DECIMAL(18,8) NOT NULL,
    "CoinsAmount" DECIMAL(18,2) NOT NULL,
    "Status" INTEGER NOT NULL,
    "RequestedAt" TIMESTAMP WITH TIME ZONE NOT NULL,
    "ConfirmedAt" TIMESTAMP WITH TIME ZONE,
    "BlockchainExplorerUrl" VARCHAR(500),
    "SenderWalletAddress" VARCHAR(50)
);

-- Create indexes if they don't exist
DO $$ 
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'payments' 
        AND tablename = 'CryptoDepositRequests' 
        AND indexname = 'IX_CryptoDepositRequests_UserId'
    ) THEN
        CREATE INDEX "IX_CryptoDepositRequests_UserId" 
        ON payments."CryptoDepositRequests" ("UserId");
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'payments' 
        AND tablename = 'CryptoDepositRequests' 
        AND indexname = 'IX_CryptoDepositRequests_TransactionId'
    ) THEN
        CREATE UNIQUE INDEX "IX_CryptoDepositRequests_TransactionId" 
        ON payments."CryptoDepositRequests" ("TransactionId");
    END IF;

    IF NOT EXISTS (
        SELECT 1 FROM pg_indexes 
        WHERE schemaname = 'payments' 
        AND tablename = 'CryptoDepositRequests' 
        AND indexname = 'IX_CryptoDepositRequests_Status'
    ) THEN
        CREATE INDEX "IX_CryptoDepositRequests_Status" 
        ON payments."CryptoDepositRequests" ("Status");
    END IF;
END $$;

-- ========================================
-- 0. CLEANUP - Delete existing test data (in reverse foreign key order)
-- ========================================
DELETE FROM payments."PurchaseNotifications" WHERE "Id" < 0;
DELETE FROM payments."PaymentRecords" WHERE "Id" < 0;
DELETE FROM payments."OrderItems" WHERE "ShoppingCartId" IN (SELECT "Id" FROM payments."ShoppingCarts" WHERE "Id" < 0);
DELETE FROM payments."ShoppingCarts" WHERE "Id" < 0;
DELETE FROM payments."Coupons" WHERE "Id" < 0;
DELETE FROM payments."CryptoDepositRequests" WHERE "Id" < 0;
DELETE FROM payments."Wallets" WHERE "Id" < 0;

-- ========================================
-- 1. WALLETS - Test wallets for tourists
-- ========================================
INSERT INTO payments."Wallets" ("Id", "UserId", "Balance", "CreatedAt", "SolanaWalletAddress")
VALUES
    (-1, -21, 10000.00, NOW(), NULL),
    (-2, -22, 5000.00, NOW(), NULL),
    (-3, -23, 100.00, NOW(), NULL);

-- ========================================
-- 2. COUPONS - Test coupons for authors
-- ========================================
INSERT INTO payments."Coupons" 
    ("Id", "Code", "DiscountPercentage", "ExpirationDate", "AuthorId", "TourId", "IsActive")
VALUES 
    -- Author -11 coupons (4 total for testing)
    (-1, 'TESTCP01', 20, NOW() + INTERVAL '6 months', -11, NULL, true),
    (-2, 'TESTCP02', 10, NOW() + INTERVAL '1 month', -11, NULL, true),
    (-3, 'EXPIRED1', 50, NOW() - INTERVAL '10 days', -11, NULL, true),
    (-4, 'INACTIVE', 15, NOW() + INTERVAL '1 year', -11, NULL, false),
    
    -- Author -12 coupons
    (-5, 'AUTHOR12', 25, NULL, -12, NULL, true),
    (-6, 'TOUR2CPN', 30, NOW() + INTERVAL '2 months', -12, NULL, true),
    
    -- Author -13 coupons
    (-7, 'AUTHOR13', 40, NOW() + INTERVAL '3 months', -13, NULL, true);

-- ========================================
-- 3. PAYMENT RECORDS - Historical purchases
-- ========================================
INSERT INTO payments."PaymentRecords"
    ("Id", "TouristId", "TourId", "BundleId", "OriginalPrice", "DiscountPercentage", "FinalPrice", "PurchaseDate", "CouponCode")
VALUES 
    (-1, -21, -1, NULL, 1500.00, 0, 1500.00, NOW() - INTERVAL '10 days', NULL),
    (-2, -21, -3, NULL, 3500.00, 20, 2800.00, NOW() - INTERVAL '5 days', 'TESTCP01'),
    (-3, -22, -1, NULL, 1500.00, 10, 1350.00, NOW() - INTERVAL '3 days', 'TESTCP02');

-- ========================================
-- 4. PURCHASE NOTIFICATIONS - Sample notifications
-- ========================================
INSERT INTO payments."PurchaseNotifications" ("Id", "TouristId", "Message", "IsRead", "CreatedAt")
VALUES
    (-1, -21, 'Successfully purchased "Obilazak Petrovaradinske tvr?ave"', false, NOW() - INTERVAL '10 days'),
    (-2, -21, 'Successfully purchased "Dunav - vožnja brodom" with 20% discount', true, NOW() - INTERVAL '5 days');

-- ========================================
-- TEST DATA SUMMARY
-- ========================================
-- Wallets: 3 test wallets for tourists -21, -22, -23
-- Coupons: 7 test coupons
--   Author -11: 4 coupons (2 active general, 1 expired, 1 inactive)
--   Author -12: 2 coupons
--   Author -13: 1 coupon
-- Payment Records: 3 historical purchases
-- Purchase Notifications: 2 sample notifications
-- CryptoDepositRequests table: Created and ready
-- ========================================
