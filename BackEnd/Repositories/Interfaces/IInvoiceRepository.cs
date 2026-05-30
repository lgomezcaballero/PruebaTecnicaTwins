using BackEnd.Dtos;

namespace BackEnd.Repositories.Interfaces;

public interface IInvoiceRepository
{
	Task<int> CreateAsync(CreateInvoiceDto invoice);
	Task<IReadOnlyList<InvoiceDto>> GetAllAsync();
	Task<InvoiceDto?> GetByIdAsync(int id);
}