using ServerlessCqrs.Host.Modules.Accounts;
using ServerlessCqrs.Host.Modules.Transactions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddDapr();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAccountComponents();
builder.Services.AddTransactionComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseCloudEvents();
app.MapControllers();
app.MapSubscribeHandler();

app.Run();
