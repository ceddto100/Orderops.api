-- ============================================================
-- OrderOps API - Useful SQL Queries for Learning
-- Run these against the OrderOpsDb database in SSMS or Azure Data Studio
-- ============================================================

-- 1. Orders with Customer Names
-- Shows how JOIN connects related tables
SELECT
    o.Id AS OrderId,
    c.Name AS CustomerName,
    c.Email,
    o.OrderDate,
    o.Status,
    o.TotalAmount
FROM Orders o
INNER JOIN Customers c ON o.CustomerId = c.Id
ORDER BY o.OrderDate DESC;

-- 2. Daily Revenue Report
-- Uses GROUP BY with aggregate functions
SELECT
    CAST(p.CreatedAt AS DATE) AS PaymentDate,
    COUNT(DISTINCT p.OrderId) AS OrderCount,
    SUM(p.Amount) AS TotalRevenue
FROM Payments p
WHERE p.Status = 'Completed'
GROUP BY CAST(p.CreatedAt AS DATE)
ORDER BY PaymentDate DESC;

-- 3. Top-Selling Products
-- Joins OrderItems to Products and ranks by quantity sold
SELECT
    pr.Id AS ProductId,
    pr.Name AS ProductName,
    pr.SKU,
    SUM(oi.Quantity) AS TotalQuantitySold,
    SUM(oi.Quantity * oi.UnitPrice) AS TotalRevenue
FROM OrderItems oi
INNER JOIN Products pr ON oi.ProductId = pr.Id
GROUP BY pr.Id, pr.Name, pr.SKU
ORDER BY TotalQuantitySold DESC;

-- 4. Customers with Most Orders
-- COUNT with GROUP BY and HAVING
SELECT
    c.Id AS CustomerId,
    c.Name,
    c.Email,
    COUNT(o.Id) AS OrderCount,
    SUM(o.TotalAmount) AS TotalSpent
FROM Customers c
LEFT JOIN Orders o ON c.Id = o.CustomerId
GROUP BY c.Id, c.Name, c.Email
HAVING COUNT(o.Id) > 0
ORDER BY OrderCount DESC;

-- 5. Refund Totals by Payment
-- Shows refunds against each payment
SELECT
    p.Id AS PaymentId,
    p.OrderId,
    p.Amount AS PaymentAmount,
    p.Status AS PaymentStatus,
    COALESCE(SUM(r.Amount), 0) AS TotalRefunded,
    p.Amount - COALESCE(SUM(r.Amount), 0) AS NetAmount
FROM Payments p
LEFT JOIN Refunds r ON p.Id = r.PaymentId
GROUP BY p.Id, p.OrderId, p.Amount, p.Status
ORDER BY p.Id;

-- 6. Refund Totals by Order
-- Aggregates refunds at the order level
SELECT
    o.Id AS OrderId,
    c.Name AS CustomerName,
    o.TotalAmount AS OrderTotal,
    COALESCE(SUM(r.Amount), 0) AS TotalRefunded,
    o.TotalAmount - COALESCE(SUM(r.Amount), 0) AS NetRevenue,
    o.Status
FROM Orders o
INNER JOIN Customers c ON o.CustomerId = c.Id
LEFT JOIN Payments p ON o.Id = p.OrderId
LEFT JOIN Refunds r ON p.Id = r.PaymentId
GROUP BY o.Id, c.Name, o.TotalAmount, o.Status
ORDER BY o.Id;

-- 7. Order Details - Full Breakdown
-- Multi-table join showing complete order information
SELECT
    o.Id AS OrderId,
    c.Name AS CustomerName,
    pr.Name AS ProductName,
    oi.Quantity,
    oi.UnitPrice,
    (oi.Quantity * oi.UnitPrice) AS LineTotal,
    o.Status
FROM Orders o
INNER JOIN Customers c ON o.CustomerId = c.Id
INNER JOIN OrderItems oi ON o.Id = oi.OrderId
INNER JOIN Products pr ON oi.ProductId = pr.Id
ORDER BY o.Id, pr.Name;

-- 8. Low Stock Alert
-- Products running low on inventory
SELECT
    Name,
    SKU,
    StockQuantity,
    Price
FROM Products
WHERE StockQuantity < 20
ORDER BY StockQuantity ASC;

-- 9. Unpaid Orders
-- Orders still in Pending status
SELECT
    o.Id AS OrderId,
    c.Name AS CustomerName,
    o.TotalAmount,
    o.OrderDate,
    DATEDIFF(DAY, o.OrderDate, GETDATE()) AS DaysUnpaid
FROM Orders o
INNER JOIN Customers c ON o.CustomerId = c.Id
WHERE o.Status = 'Pending'
ORDER BY o.OrderDate ASC;

-- 10. Monthly Revenue Trend
-- Revenue grouped by month for trend analysis
SELECT
    YEAR(p.CreatedAt) AS PaymentYear,
    MONTH(p.CreatedAt) AS PaymentMonth,
    COUNT(DISTINCT p.OrderId) AS OrderCount,
    SUM(p.Amount) AS MonthlyRevenue
FROM Payments p
WHERE p.Status = 'Completed'
GROUP BY YEAR(p.CreatedAt), MONTH(p.CreatedAt)
ORDER BY PaymentYear DESC, PaymentMonth DESC;
