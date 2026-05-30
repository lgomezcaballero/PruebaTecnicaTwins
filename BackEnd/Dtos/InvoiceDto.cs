namespace BackEnd.Dtos;

public class InvoiceDto
{
	public InvoiceDto()
	{
		this.InvoiceNumber = string.Empty;
		this.CustomerName = string.Empty;
		this.Items = new List<InvoiceItemDto>();
	}
	public int Id { get; set; }
	public string InvoiceNumber { get; set; }
	public DateTime InvoiceDate { get; set; }
	public string CustomerName { get; set; }
	public string? CustomerDocument { get; set; }
	public decimal NetAmount { get; set; }
	public decimal VatAmount { get; set; }
	public decimal TotalAmount { get; set; }
	public DateTime CreatedAt { get; set; }
	public List<InvoiceItemDto> Items { get; set; }
}