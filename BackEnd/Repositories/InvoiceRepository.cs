using BackEnd.Dtos;
using BackEnd.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;

namespace BackEnd.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
	private const string CreateInvoiceProcedure = "dbo.sp_invoice_create";
	private const string GetInvoicesProcedure = "dbo.sp_get_invoice";

	private readonly string _connectionString;

	public InvoiceRepository(IConfiguration configuration)
	{
		_connectionString = configuration.GetConnectionString("DefaultConnection")
			?? throw new InvalidOperationException("DefaultConnection was not configured.");
	}

	public async Task<int> CreateAsync(CreateInvoiceDto invoice)
	{
		ArgumentNullException.ThrowIfNull(invoice);

		//Si bien el SP tiene validaciones, consideré que podemos tener algunas minimas validaciones del lado del servicio asi 
		//no llamamos innecesariamente a la base si no contamos con los datos minimos.
		if (invoice.Items is null || invoice.Items.Count == 0)
			throw new ArgumentException("La factura debe tener al menos un item.", nameof(invoice));

		SqlConnection connection = new SqlConnection(_connectionString);
		SqlCommand command = new SqlCommand(CreateInvoiceProcedure, connection)
		{
			CommandType = CommandType.StoredProcedure
		};

		command.Parameters.Add("@InvoiceNumber", SqlDbType.VarChar, 50).Value = invoice.InvoiceNumber.Trim();
		command.Parameters.Add("@InvoiceDate", SqlDbType.Date).Value = invoice.InvoiceDate.Date;
		command.Parameters.Add("@CustomerName", SqlDbType.VarChar, 150).Value = invoice.CustomerName.Trim();

		var customerDocument = string.IsNullOrWhiteSpace(invoice.CustomerDocument)
			? (object)DBNull.Value
			: invoice.CustomerDocument.Trim();

		command.Parameters.Add("@CustomerDocument", SqlDbType.VarChar, 50).Value = customerDocument;

		//Creamos un dataset a ser enviado como parametro
		var itemsParameter = command.Parameters.Add("@Items", SqlDbType.Structured);
		itemsParameter.TypeName = "dbo.InvoiceItemTableType";
		itemsParameter.Value = CreateItemsDataTable(invoice.Items);

		var invoiceIdParameter = command.Parameters.Add("@InvoiceId", SqlDbType.Int);
		invoiceIdParameter.Direction = ParameterDirection.Output;

		await connection.OpenAsync();
		await command.ExecuteNonQueryAsync();

		return Convert.ToInt32(invoiceIdParameter.Value);
	}

	public async Task<IReadOnlyList<InvoiceDto>> GetAllAsync()
	{
		return await GetInvoicesAsync(null);
	}

	public async Task<InvoiceDto?> GetByIdAsync(int id)
	{
		var invoices = await GetInvoicesAsync(id);

		return invoices.FirstOrDefault();
	}

	private async Task<List<InvoiceDto>> GetInvoicesAsync(int? id)
	{
		SqlConnection connection = new SqlConnection(_connectionString);
		SqlCommand command = new SqlCommand(GetInvoicesProcedure, connection)
		{
			CommandType = CommandType.StoredProcedure
		};

		command.Parameters.Add("@InvoiceId", SqlDbType.Int).Value = id.HasValue ? id.Value : DBNull.Value;

		await connection.OpenAsync();
		var reader = await command.ExecuteReaderAsync();

		var idOrdinal = reader.GetOrdinal("Id");
		var invoiceNumberOrdinal = reader.GetOrdinal("InvoiceNumber");
		var invoiceDateOrdinal = reader.GetOrdinal("InvoiceDate");
		var customerNameOrdinal = reader.GetOrdinal("CustomerName");
		var customerDocumentOrdinal = reader.GetOrdinal("CustomerDocument");
		var netAmountOrdinal = reader.GetOrdinal("NetAmount");
		var vatAmountOrdinal = reader.GetOrdinal("VatAmount");
		var totalAmountOrdinal = reader.GetOrdinal("TotalAmount");
		var createdAtOrdinal = reader.GetOrdinal("CreatedAt");
		var itemIdOrdinal = reader.GetOrdinal("ItemId");
		var descriptionOrdinal = reader.GetOrdinal("Description");
		var quantityOrdinal = reader.GetOrdinal("Quantity");
		var unitPriceOrdinal = reader.GetOrdinal("UnitPrice");
		var vatRateOrdinal = reader.GetOrdinal("VatRate");
		var itemNetAmountOrdinal = reader.GetOrdinal("ItemNetAmount");
		var itemVatAmountOrdinal = reader.GetOrdinal("ItemVatAmount");
		var itemTotalAmountOrdinal = reader.GetOrdinal("ItemTotalAmount");

		var invoices = new Dictionary<int, InvoiceDto>();

		while (await reader.ReadAsync())
		{
			var invoiceId = reader.GetInt32(idOrdinal);

			if (!invoices.TryGetValue(invoiceId, out var invoice))
			{
				invoice = new InvoiceDto
				{
					Id = invoiceId,
					InvoiceNumber = reader.GetString(invoiceNumberOrdinal),
					InvoiceDate = reader.GetDateTime(invoiceDateOrdinal),
					CustomerName = reader.GetString(customerNameOrdinal),
					CustomerDocument = reader.IsDBNull(customerDocumentOrdinal)
						? null
						: reader.GetString(customerDocumentOrdinal),
					NetAmount = reader.GetDecimal(netAmountOrdinal),
					VatAmount = reader.GetDecimal(vatAmountOrdinal),
					TotalAmount = reader.GetDecimal(totalAmountOrdinal),
					CreatedAt = reader.GetDateTime(createdAtOrdinal)
				};

				invoices.Add(invoiceId, invoice);
			}

			if (!reader.IsDBNull(itemIdOrdinal))
			{
				invoice.Items.Add(new InvoiceItemDto
				{
					Id = reader.GetInt32(itemIdOrdinal),
					Description = reader.GetString(descriptionOrdinal),
					Quantity = reader.GetDecimal(quantityOrdinal),
					UnitPrice = reader.GetDecimal(unitPriceOrdinal),
					VatRate = reader.GetDecimal(vatRateOrdinal),
					NetAmount = reader.GetDecimal(itemNetAmountOrdinal),
					VatAmount = reader.GetDecimal(itemVatAmountOrdinal),
					TotalAmount = reader.GetDecimal(itemTotalAmountOrdinal)
				});
			}
		}

		return invoices.Values.ToList();
	}

	private static DataTable CreateItemsDataTable(IEnumerable<CreateInvoiceItemDto> items)
	{
		var table = new DataTable();

		table.Columns.Add("Description", typeof(string));
		table.Columns.Add("Quantity", typeof(decimal));
		table.Columns.Add("UnitPrice", typeof(decimal));
		table.Columns.Add("VatRate", typeof(decimal));

		foreach (var item in items)
		{
			table.Rows.Add(
				item.Description.Trim(),
				item.Quantity,
				item.UnitPrice,
				item.VatRate);
		}

		return table;
	}
}