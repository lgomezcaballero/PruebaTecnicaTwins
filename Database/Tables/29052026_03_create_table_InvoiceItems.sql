--Items de la factura
IF OBJECT_ID('dbo.InvoiceItems', 'U') IS NULL
	CREATE TABLE dbo.InvoiceItems
    (
        Id INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        InvoiceId INT NOT NULL,
        Description VARCHAR(200) NOT NULL,
        Quantity DECIMAL(18,2) NOT NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,
        VatRate DECIMAL(5,2) NOT NULL,
        NetAmount DECIMAL(18,2) NOT NULL,
        VatAmount DECIMAL(18,2) NOT NULL,
        TotalAmount DECIMAL(18,2) NOT NULL,

        CONSTRAINT FK_InvoiceItems_Invoices FOREIGN KEY (InvoiceId) REFERENCES Invoices(Id)
    );
GO