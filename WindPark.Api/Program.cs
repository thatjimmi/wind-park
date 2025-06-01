using FluentValidation;
using WindPark.Features.TurbinePark.AdjustProductionTarget;
using WindPark.Features.TurbinePark.GetParkStatus;
using WindPark.Features.TurbinePark.SetMarketPrice;
using WindPark.Infrastructure.Repositories;
using WindPark.Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddSingleton<ITurbineParkRepository, InMemoryTurbineParkRepository>();
builder.Services.AddScoped<SetMarketPriceHandler>();
builder.Services.AddScoped<AdjustProductionTargetHandler>();
builder.Services.AddScoped<GetParkStatusHandler>();

var app = builder.Build();

app.MapEndpoints();

app.Run();
