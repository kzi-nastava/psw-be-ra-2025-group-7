-- Added payment-related seed data for Wallets and PaymentRecords
-- Run after the original seed to ensure wallets and payment records exist

-- Ensure payments schema exists
CREATE SCHEMA IF NOT EXISTS payments;

-- Wallets (ensure users -21, -22, -23 exist in stakeholders.Users)
CREATE TABLE IF NOT EXISTS payments."Wallets" (
    "Id" BIGSERIAL PRIMARY KEY,
    "UserId" BIGINT NOT NULL UNIQUE,
    "Balance" NUMERIC(18,2) NOT NULL,
    "CreatedAt" TIMESTAMPTZ NOT NULL
);

-- PaymentRecords table for audit
CREATE TABLE IF NOT EXISTS payments."PaymentRecords" (
    "Id" BIGSERIAL PRIMARY KEY,
    "UserId" BIGINT NOT NULL,
    "TourId" BIGINT NULL,
    "BundleId" BIGINT NULL,
    "Amount" NUMERIC(18,2) NOT NULL,
    "CreatedAt" TIMESTAMPTZ NOT NULL,
    "Description" TEXT NULL
);

-- Insert wallets for seeded tourists (ids used in tests and seed data)
INSERT INTO payments."Wallets" ("Id", "UserId", "Balance", "CreatedAt") VALUES
    (-1, -21, 10000.00, NOW()),
    (-2, -22, 500.00, NOW()),
    (-3, -23, 0.00, NOW())
ON CONFLICT ("Id") DO NOTHING;

-- Optional: seed a few payment records to reflect historic purchases
INSERT INTO payments."PaymentRecords" ("UserId", "TourId", "Amount", "CreatedAt", "Description") VALUES
    (-1, -3, 3500.00, NOW(), 'Historic purchase: Dunav - vožnja brodom'),
    (-21, -1, 1500.00, NOW(), 'Obilazak Petrovaradinske tvr?ave')
ON CONFLICT DO NOTHING;
