--Tabla de items que será usada para enviar los items de la factura, esto permite hacer un solo llamado a la base por factura
--y que no se haga x llamados por x cantidad de items que tenga una factura, como me ha tocado ver varias veces
IF TYPE_ID('dbo.InvoiceItemTableType') IS NULL
	CREATE TYPE dbo.InvoiceItemTableType AS TABLE
    (
        Description VARCHAR(200) NOT NULL,
        Quantity DECIMAL(18,2) NOT NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,
        VatRate DECIMAL(5,2) NOT NULL
    );
GO