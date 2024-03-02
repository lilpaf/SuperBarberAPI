using Business.Configurations;
using Business.Implementations;
using Business.Interfaces;
using Business.Models.Exceptions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistence.Contexts;
using Persistence.Entities;
using Persistence.Implementations;
using Persistence.Interfaces;
using SuperBarber.Extensions;
using SuperBarber.Filters;
using SuperBarber.Middlewares;
using System.Text;
using BarberShopService = Business.Implementations.BarberShopService;

var builder = WebApplication.CreateBuilder(args);

/*
 * ToDo check if email is confirmed before making orders or registering as barber
 */
// Add services to the container.

builder.Services.AddDbContext<SuperBarberDbContext>(
            optionsBuilder => optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("SuperBarber")));

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection(nameof(JwtConfig)));
builder.Services.Configure<SmtpConfig>(builder.Configuration.GetSection(nameof(SmtpConfig)));
builder.Services.Configure<IdentityConfig>(builder.Configuration.GetSection(nameof(IdentityConfig)));

builder.AddCustomAuthentication();

builder.Services.AddHttpContextAccessor();

//This is needed in order to use the ValidateModelStateFilter
builder.Services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IBarberShopService, BarberShopService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBarberShopRepository, BarberShopRepository>();

//Filters
builder.Services.AddMvc(options =>
{
    options.Filters.Add<ValidateModelStateFilter>(0);
});

var app = builder.Build();

app.PrepareDataBase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

// Middlewares
app.UseMiddleware<LogMetaDataMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();


