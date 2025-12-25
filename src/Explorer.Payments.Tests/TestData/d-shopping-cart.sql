-- ============================================================
-- d-shopping-cart.sql
-- Test data for ShoppingCart functionality in Payments schema
-- ============================================================

-- ------------------------------------------------------------
-- Shopping Carts for testing
-- ------------------------------------------------------------
-- Shopping cart for tourist ID=1 with items (for testing GetMyCart)
INSERT INTO payments."ShoppingCarts" ("Id", "TouristId", "TotalPrice")
VALUES 
    (-1, 1, 220.00);

-- Items in cart for tourist 1
INSERT INTO payments."OrderItems" ("Id", "TourId", "TourName", "Price", "ShoppingCartId")
VALUES 
    (-1, -3, 'Test Tour 3 - Published', 100.00, -1),
    (-2, -5, 'Test Tour 5 - Published for Reactivate', 120.00, -1);

-- ------------------------------------------------------------
-- NOTES FOR TESTS:
-- ------------------------------------------------------------
-- Tests use the following TouristId values: 1, 2, 3, 4, 5, 6, 7
-- Published tours available for adding to cart: -3, -5, -10, -11 (defined in Tours test data)
-- Archived tour (cannot add to cart): -100 (defined in Tours test data)
-- Draft tour (cannot add to cart): -101 (defined in Tours test data)
-- ------------------------------------------------------------
