using FluentValidation;
using FluentValidation.AspNetCore;
using Stock.API.Core.Entities;
using Stock.API.Core.Validators;
using Stock.API.IOC;
using Stock.API.Web.DTOs;
using Stock.API.Web.Middlewares;
using Stock.API.Web.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IValidator<Product>, ProductValidator>();
builder.Services.AddScoped<IValidator<ProductDTO>, ProductDTOValidator>();

builder.Services.InjectRepositories(builder.Configuration);
builder.Services.InjectRabbitMQ(builder.Configuration);
builder.Services.InjectServices();
builder.Services.InjectValidators();
builder.Services.AddValidatorsFromAssemblyContaining<ProductValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ProductDTOValidator>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();