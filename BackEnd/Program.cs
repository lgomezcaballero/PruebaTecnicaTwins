using BackEnd.Repositories;
using BackEnd.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);
const string ReactAppCorsPolicy = "ReactApp";

if (builder.Environment.IsDevelopment())
{
	builder.Configuration.AddUserSecrets<Program>(optional: true);
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
//Hice algunas modificaciones acá porque queria levantar localmente la api y el front react para validar que le pegara correctamente
builder.Services.AddCors(options =>
{
	options.AddPolicy(ReactAppCorsPolicy, policy =>
	{
		policy
			.WithOrigins(
				"http://localhost:5173",
				"http://127.0.0.1:5173")
			.AllowAnyHeader()
			.AllowAnyMethod();
	});
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	//Aca lo mismo, configuro swagger para poder probar manualmente los endpoints
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(ReactAppCorsPolicy);

app.UseAuthorization();

app.MapControllers();

app.Run();