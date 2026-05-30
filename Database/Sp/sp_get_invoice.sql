--Obtiene las facturas, parametro opcional permitiendo filtrar por id o traer todos
CREATE OR ALTER PROCEDURE dbo.sp_get_invoice
(
    @InvoiceId INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        i.Id,
        i.InvoiceNumber,
        i.InvoiceDate,
        i.CustomerName,
        i.CustomerDocument,
        i.NetAmount,
        i.VatAmount,
        i.TotalAmount,
        i.CreatedAt,
        ii.Id AS ItemId,
        ii.Description,
        ii.Quantity,
        ii.UnitPrice,
        ii.VatRate,
        ii.NetAmount AS ItemNetAmount,
        ii.VatAmount AS ItemVatAmount,
        ii.TotalAmount AS ItemTotalAmount
    FROM dbo.Invoices i
    LEFT JOIN dbo.InvoiceItems ii ON ii.InvoiceId = i.Id
    WHERE @InvoiceId IS NULL OR i.Id = @InvoiceId
    ORDER BY i.Id DESC, ii.Id ASC;
END
GO