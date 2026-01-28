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

DO $$
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM information_schema.columns
        WHERE table_schema = 'payments'
          AND table_name = 'Coupons'
          AND column_name = 'IsUniversal'
    ) THEN
        ALTER TABLE payments."Coupons"
        ADD COLUMN "IsUniversal" boolean NOT NULL DEFAULT false;
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

DELETE FROM payments."SaleTours" WHERE "SaleId" < 0;
DELETE FROM payments."Sales" WHERE "Id" < 0;

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
("Id","Code","DiscountPercentage","ExpirationDate","AuthorId","TourId","IsActive","IsUniversal")
VALUES 
    (-1,'TESTCP01',20, NOW() + INTERVAL '6 months', -11, NULL, true, false),
    (-2,'TESTCP02',10, NOW() + INTERVAL '1 month',  -11, NULL, true, false),
    (-3,'EXPIRED1',50, NOW() - INTERVAL '10 days',  -11, NULL, true, false),
    (-4,'INACTIVE',15, NOW() + INTERVAL '1 year',   -11, NULL, false, false),

    (-5,'AUTHOR12',25, NULL, -12, NULL, true, false),
    (-6,'TOUR2CPN',30, NOW() + INTERVAL '2 months', -12, NULL, true, false),

    (-7,'AUTHOR13',40, NOW() + INTERVAL '3 months', -13, NULL, true, false),

    -- NEW: universal issued by -11
    (-8,'UNIV10AA',10, NULL, -11, NULL, true, true);


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
-- 5. SALES - Test sales for authors
-- ========================================
INSERT INTO payments."Sales"
    ("Id", "AuthorId", "Start", "End", "DiscountPercentage", "Status")
VALUES
    -- Author -11 sales
    (-1, -11, NOW() - INTERVAL '1 day', NOW() + INTERVAL '5 days', 20, 0), -- Draft
    (-2, -11, NOW() - INTERVAL '2 days', NOW() + INTERVAL '3 days', 30, 1), -- Active
    (-3, -11, NOW() - INTERVAL '10 days', NOW() - INTERVAL '1 day', 15, 2), -- Expired

    -- Author -12 sale
    (-4, -12, NOW(), NOW() + INTERVAL '7 days', 25, 0);


-- ========================================
-- 6. SALE TOURS - Relations between sales and tours
-- ========================================
INSERT INTO payments."SaleTours"
    ("SaleId", "TourId")
VALUES
    -- Sale -1 (Draft)
    (-1, -1),
    (-1, -2),

    -- Sale -2 (Active)
    (-2, -1),

    -- Sale -3 (Expired)
    (-3, -3),

    -- Sale -4 (Author -12)
    (-4, -2);

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
