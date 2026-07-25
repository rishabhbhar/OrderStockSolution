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
