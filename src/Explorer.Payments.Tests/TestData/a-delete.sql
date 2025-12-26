-- ============================================================
-- a-delete.sql
-- Cleanup script for Payments test data
-- ============================================================

-- Delete all OrderItems (child records first)
DELETE FROM payments."OrderItems";

-- Delete all ShoppingCarts
DELETE FROM payments."ShoppingCarts";

DELETE FROM payments."Wallets";
