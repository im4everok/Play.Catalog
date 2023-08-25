using Play.Catalog.Service.Entities;
using Play.Common.MassTransit;
using Play.Common.MongoDb;
using Play.Common.Settings;

var builder = WebApplication.CreateBuilder(args);
ServiceSettings serviceSettings;

// Add services to the container.

var services = builder.Services;
var configuration = builder.Configuration;

services.AddControllers(options =>
{
    options.SuppressAsyncSuffixInActionNames = false;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

services.AddMongo().AddMongoRepository<Item>("items")
    .AddMassTransitWithRabbitMq();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
