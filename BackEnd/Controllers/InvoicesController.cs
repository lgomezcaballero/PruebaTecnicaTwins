using BackEnd.Dtos;
using BackEnd.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
	private readonly IInvoiceRepository _invoices;

	public InvoicesController(IInvoiceRepository invoices)
	{
		this._invoices = invoices;
	}

	[HttpGet]
	public async Task<ActionResult<IReadOnlyList<InvoiceDto>>> GetAll()
	{
		var invoices = await _invoices.GetAllAsync();

		return Ok(invoices);
	}

	[HttpGet("{id:int}")]
	public async Task<ActionResult<InvoiceDto>> GetById(int id)
	{
		var invoice = await _invoices.GetByIdAsync(id);

		return invoice == null ? NotFound() : Ok(invoice);
	}

	[HttpPost]
	public async Task<ActionResult> Create([FromBody] CreateInvoiceDto invoice)
	{
		try
		{
			var invoiceId = await _invoices.CreateAsync(invoice);

			return CreatedAtAction(nameof(GetById), new { id = invoiceId }, new
			{
				id = invoiceId,
				message = "Factura creada correctamente."
			});
		}
		catch (SqlException ex)
		{
			return BadRequest(new
			{
				message = ex.Message
			});
		}
	}
}