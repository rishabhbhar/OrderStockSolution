/* =============================================================
   Product Service Database (SQL Server)
   ============================================================= */
IF DB_ID('ProductDb') IS NULL
BEGIN
    CREATE DATABASE ProductDb;
END
GO

USE ProductDb;
GO

IF OBJECT_ID('dbo.Products', 'U') IS NOT NULL
    DROP TABLE dbo.Products;
GO

CREATE TABLE dbo.Products
(
    ProductId     UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Products_ProductId DEFAULT NEWID(),
    ProductName   NVARCHAR(150)    NOT NULL,
    Price         DECIMAL(10,2)    NOT NULL CONSTRAINT CK_Products_Price CHECK (Price >= 0),
    StockQty      INT              NOT NULL CONSTRAINT CK_Products_StockQty CHECK (StockQty >= 0),
    IsActive      BIT              NOT NULL CONSTRAINT DF_Products_IsActive DEFAULT (1),
    CreatedAt     DATETIME2        NOT NULL CONSTRAINT DF_Products_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt     DATETIME2        NULL,
    RowVersion    ROWVERSION       NOT NULL,
    CONSTRAINT PK_Products PRIMARY KEY (ProductId)
);
GO

CREATE INDEX IX_Products_Name ON dbo.Products (ProductName);
GO

-- Sample seed data (optional - remove if not needed)
INSERT INTO dbo.Products (ProductId, ProductName, Price, StockQty, IsActive)
VALUES
    (NEWID(), N'Wireless Mouse', 799.00, 150, 1),
    (NEWID(), N'Mechanical Keyboard', 3499.00, 75, 1),
    (NEWID(), N'27-inch Monitor', 15999.00, 30, 1);
GO
