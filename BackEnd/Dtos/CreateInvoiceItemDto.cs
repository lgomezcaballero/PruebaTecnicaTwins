using System.ComponentModel.DataAnnotations;

namespace BackEnd.Dtos;

public class CreateInvoiceItemDto
{
	public CreateInvoiceItemDto()
	{
		this.Description = string.Empty;
	}
	[Required(ErrorMessage = "La descripcion es requerida.")]
	[StringLength(200, ErrorMessage = "La descripcion no puede superar los 200 caracteres.")]
	public string Description { get; set; }

	[Range(typeof(decimal), "0.01", "9999999999999999", ErrorMessage = "La cantidad debe ser mayor a cero.")]
	public decimal Quantity { get; set; }

	[Range(typeof(decimal), "0", "9999999999999999", ErrorMessage = "El precio unitario no puede ser negativo.")]
	public decimal UnitPrice { get; set; }

	[Range(typeof(decimal), "0", "100", ErrorMessage = "El IVA debe estar entre 0 y 100.")]
	public decimal VatRate { get; set; }
}