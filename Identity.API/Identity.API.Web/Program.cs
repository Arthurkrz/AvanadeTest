using FluentValidation;
using FluentValidation.AspNetCore;
using Identity.API.Core.Validators;
using Identity.API.IOC;
using Identity.API.Web.DTOs;
using Identity.API.Web.Utilities;
using Identity.API.Web.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var jwtSettings = builder.Configuration.GetSection("Jwt");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    });

builder.Services.AddScoped<IValidator<AdminDTO>, AdminDTOValidator>();
builder.Services.AddScoped<IValidator<BuyerDTO>, BuyerDTOValidator>();

builder.Services.InjectRepositories(builder.Configuration);
builder.Services.InjectServices();
builder.Services.InjectValidators();
builder.Services.AddValidatorsFromAssemblyContaining<AdminDTOValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AdminRegisterRequestValidator>();

builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();