--Creación de factura, también contiene validaciones de negocio
CREATE OR ALTER PROCEDURE dbo.sp_invoice_create
(
    @InvoiceNumber VARCHAR(50),
    @InvoiceDate DATE,
    @CustomerName VARCHAR(150),
    @CustomerDocument VARCHAR(50) = NULL,
    @Items dbo.InvoiceItemTableType READONLY,
    @InvoiceId INT OUTPUT
)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        IF NULLIF(LTRIM(RTRIM(@InvoiceNumber)), '') IS NULL
            THROW 50001, 'El número de factura es obligatorio.', 1;

        IF NULLIF(LTRIM(RTRIM(@CustomerName)), '') IS NULL
            THROW 50002, 'El cliente es obligatorio.', 1;

        IF EXISTS (SELECT 1 FROM dbo.Invoices WHERE InvoiceNumber = @InvoiceNumber)
            THROW 50003, 'Ya existe una factura con ese número.', 1;

        IF NOT EXISTS (SELECT 1 FROM @Items)
            THROW 50004, 'La factura debe tener al menos un ítem.', 1;

        IF EXISTS (SELECT 1 FROM @Items WHERE 
                NULLIF(LTRIM(RTRIM(Description)), '') IS NULL
                OR Quantity <= 0
                OR UnitPrice < 0
                OR VatRate < 0
        )
            THROW 50005, 'Uno o más ítems tienen datos inválidos.', 1;

        DECLARE @NetAmount DECIMAL(18,2);
        DECLARE @VatAmount DECIMAL(18,2);
        DECLARE @TotalAmount DECIMAL(18,2);

        SELECT
            @NetAmount = ROUND(SUM(Quantity * UnitPrice), 2),
            @VatAmount = ROUND(SUM((Quantity * UnitPrice) * VatRate / 100), 2),
            @TotalAmount = ROUND(SUM((Quantity * UnitPrice) + ((Quantity * UnitPrice) * VatRate / 100)), 2)
        FROM @Items;    

        INSERT INTO dbo.Invoices (InvoiceNumber, InvoiceDate, CustomerName, CustomerDocument, NetAmount, VatAmount, TotalAmount)
        VALUES(@InvoiceNumber, @InvoiceDate, @CustomerName, @CustomerDocument, @NetAmount, @VatAmount, @TotalAmount)

        SET @InvoiceId = CAST(SCOPE_IDENTITY() AS INT);

        INSERT INTO dbo.InvoiceItems (InvoiceId, Description, Quantity, UnitPrice, VatRate, NetAmount, VatAmount, TotalAmount)
        SELECT @InvoiceId, Description, Quantity, UnitPrice, VatRate, ROUND(Quantity * UnitPrice, 2), ROUND((Quantity * UnitPrice) * VatRate / 100, 2),
            ROUND((Quantity * UnitPrice) + ((Quantity * UnitPrice) * VatRate / 100), 2)
        FROM @Items;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0
            ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END
GO