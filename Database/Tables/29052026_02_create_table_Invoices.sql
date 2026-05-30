--Consideramos hacer una pk compuesta con invoiceNumber pero podria repetirse el invoiceNumber y no es consistente
IF OBJECT_ID('dbo.Invoices', 'U') IS NULL
    CREATE TABLE dbo.Invoices
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        InvoiceNumber VARCHAR(50) NOT NULL,
        InvoiceDate DATE NOT NULL,
        CustomerName VARCHAR(150) NOT NULL,
        CustomerDocument VARCHAR(50) NULL,
        NetAmount DECIMAL(18,2) NOT NULL,
        VatAmount DECIMAL(18,2) NOT NULL,
        TotalAmount DECIMAL(18,2) NOT NULL,
        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

        CONSTRAINT UQ_Invoices_InvoiceNumber UNIQUE (InvoiceNumber)
    );
GO