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


/* =============================================================
   Order Service Database (SQL Server)
   ============================================================= */
IF DB_ID('OrderDb') IS NULL
BEGIN
    CREATE DATABASE OrderDb;
END
GO

USE OrderDb;
GO

IF OBJECT_ID('dbo.Orders', 'U') IS NOT NULL
    DROP TABLE dbo.Orders;
GO

CREATE TABLE dbo.Orders
(
    OrderId       UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Orders_OrderId DEFAULT NEWID(),
    ProductId     UNIQUEIDENTIFIER NOT NULL,
    ProductName   NVARCHAR(150)    NULL,
    UnitPrice     DECIMAL(10,2)    NULL,
    Quantity      INT              NOT NULL CONSTRAINT CK_Orders_Quantity CHECK (Quantity > 0),
    OrderStatus   VARCHAR(30)      NOT NULL CONSTRAINT DF_Orders_OrderStatus DEFAULT ('CREATED'),
    CreatedAt     DATETIME2        NOT NULL CONSTRAINT DF_Orders_CreatedAt DEFAULT (SYSUTCDATETIME()),
    CONSTRAINT PK_Orders PRIMARY KEY (OrderId),
    CONSTRAINT CK_Orders_Status CHECK (OrderStatus IN ('CREATED', 'PAID', 'CANCELLED'))
);
GO

CREATE INDEX IX_Orders_ProductId ON dbo.Orders (ProductId);
GO
