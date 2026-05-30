using System.ComponentModel.DataAnnotations;

namespace BackEnd.Dtos;

public class CreateInvoiceDto
{
	public CreateInvoiceDto()
	{
		this.InvoiceNumber = string.Empty;
		this.CustomerName = string.Empty;
		this.Items = new List<CreateInvoiceItemDto>();
	}

	[Required(ErrorMessage = "El numero de factura es requerido.")]
	[StringLength(50, ErrorMessage = "El numero de factura no puede superar los 50 caracteres.")]
	public string InvoiceNumber { get; set; }

	public DateTime InvoiceDate { get; set; }

	[Required(ErrorMessage = "El cliente es requerido.")]
	[StringLength(150, ErrorMessage = "El cliente no puede superar los 150 caracteres.")]
	public string CustomerName { get; set; }

	[StringLength(50, ErrorMessage = "El documento no puede superar los 50 caracteres.")]
	public string? CustomerDocument { get; set; }

	public List<CreateInvoiceItemDto> Items { get; set; }
}