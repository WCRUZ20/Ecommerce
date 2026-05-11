using DealerEcommerce.Application.Abstractions;
using DealerEcommerce.Application.Auth.Commands;
using DealerEcommerce.Application.Auth.DTOs;
using DealerEcommerce.Application.Auth.Handlers;
using DealerEcommerce.Application.Common;
using DealerEcommerce.Application.Dealers.Commands;
using DealerEcommerce.Application.Dealers.DTOs;
using DealerEcommerce.Application.Dealers.Handlers;
using DealerEcommerce.Application.Users.Commands;
using DealerEcommerce.Application.Users.DTOs;
using DealerEcommerce.Application.Users.Handlers;
using DealerEcommerce.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese el JWT retornado por el endpoint de login."
    });
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<
    ICommandHandler<LoginCommand, Result<LoginResponseDto>>,
    LoginHandler>();

builder.Services.AddScoped<
    ICommandHandler<CreateUserCommand, Result<UserDto>>,
    CreateUserHandler>();

builder.Services.AddScoped<
    ICommandHandler<UpdateUserCommand, Result<UserDto>>,
    UpdateUserHandler>();

builder.Services.AddScoped<
    ICommandHandler<CreateDealerCommand, Result<DealerDto>>,
    CreateDealerHandler>();

builder.Services.AddScoped<
    ICommandHandler<UpdateDealerCommand, Result<DealerDto>>,
    UpdateDealerHandler>();

builder.Services.AddScoped<
    ICommandHandler<AddDealerAddressCommand, Result<DealerAddressDto>>,
    AddDealerAddressHandler>();

builder.Services.AddScoped<
    ICommandHandler<UpdateDealerAddressCommand, Result<DealerAddressDto>>,
    UpdateDealerAddressHandler>();

var jwtSecretKey = builder.Configuration["Jwt:SecretKey"];

if (string.IsNullOrWhiteSpace(jwtSecretKey))
    throw new InvalidOperationException("Jwt:SecretKey no está configurado.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecretKey)),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

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