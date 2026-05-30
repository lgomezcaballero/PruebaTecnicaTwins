namespace BackEnd.Dtos;

public class InvoiceItemDto
{
	public InvoiceItemDto()
	{
		this.Description = string.Empty;
	}
	public int Id { get; set; }
	public string Description { get; set; }
	public decimal Quantity { get; set; }
	public decimal UnitPrice { get; set; }
	public decimal VatRate { get; set; }
	public decimal NetAmount { get; set; }
	public decimal VatAmount { get; set; }
	public decimal TotalAmount { get; set; }
}